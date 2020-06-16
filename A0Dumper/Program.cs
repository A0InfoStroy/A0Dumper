// $Date: 2020-03-20 14:04:17 +0300 (Пт, 20 мар 2020) $
// $Revision: 51 $
// $Author: agalkin $
// --xml "8d21610a-b7f3-4eb5-b92b-14d99d0bdf2a"

using A0Service;
using System;
using System.Runtime.InteropServices;

namespace A0Dumper
{
    class Program
    {
        static public void Message(string s)
        {
            Console.Write(s);
        }

        static void Main(string[] args)
        {
            try
            {
                var Args = Environment.GetCommandLineArgs();

                if (Args.Length <= 1)
                {
                    new ArgHelp().Help();
                }
                else
                {
                    string Command = Args[1];
                    Command = Command.Remove(0, 2);

                    switch (Command)
                    {
                        case "help":
                            {
                                new ArgHelp().Help();
                                break;
                            }
                        case "version":
                            {
                                // Создание A0 API
                                API A0 = new API();
                                new AppVersion().Info(A0);
                                break;
                            }
                        case "list":
                            {
                                CommandExecute.Cmd cmd = A0 => new ObjectToConsole(A0.Estimate.Repo).Run();
                                new CommandExecute().Execute(cmd);

                                break;
                            }
                        case "xml":
                            {
                                // Дополнительный параметр GUID проекта для вывода
                                if (Args.Length != 3)
                                {
                                    Console.WriteLine("Ожидается GUID Проекта");
                                    break;
                                }

                                Guid PorjId = Guid.Parse(Args[2]);                                                  
                                CommandExecute.Cmd cmd = A0 =>
                                {
                                    //new ObjectToConsole(A0.Estimate.Repo.Objects, A0.Estimate.Repo.Objects.RootID).Run();
                                    new Store.XML.FileService("A0.xml", PorjId, A0.Estimate.Repo, Message).Execute();
                                    GC.Collect();
                                };
                                new CommandExecute().Execute(cmd);
                                break;
                            }
						case "nsi_bases":
							{
								CommandExecute.Cmd cmd = A0 =>
								{
									var it = new NSI.BaseIt(A0.Sys.NSI.Bases);
									while (it.Next())
									{
										var NSI = it.Current;
										Console.WriteLine("ID: {0} ({1}) - {2} {3} {4}", 
											NSI.ID, NSI.Mark, NSI.Name, NSI.ReleaseDate, NSI.ReleaseVersion);
									}	   
								};

								new CommandExecute().Execute(cmd);
								break;
							}
						default:
                            {
                                new ArgHelp().Help();
                                break;
                            }
                    }
                }

            }
            catch (Exception E)
            {
                Console.Error.WriteLine("Обшика {0}", E.Message);
            }
        }

        /// <summary>
        /// Запуск А0 API и выполнение команды
        /// </summary>
        public class CommandExecute
        {
            public delegate void Cmd(IAPI A0);

            public void Execute(Cmd cmd)
            {
                // Создание A0 API
                API A0 = new API();

                // Вывод информации об A0 API
                new AppVersion().Info(A0);

                // Параметры для установки соедиения
                var CS = new ConnectionSettings();
                // Установка соединения с БД А0
                var R = A0.Connect3(CS.ConnStr, CS.UserName, CS.Password);
                if (R != EConnectReturnCode.crcSuccess)
                    throw new Exception(string.Format("Не могу установить соединение с БД А0. Код возврата {0}", R));
                Console.WriteLine("Соединение установлено");

                cmd(A0);
                // Вывод сметных объектов на консоль
                //new ObjectToConsole(A0.Estimate.Repo.Objects, A0.Estimate.Repo.Objects.RootID).Run();

                // Вывод проекта в XML
                //Guid PorjId = Guid.Parse("e683472e-3c9c-4532-9cde-376a7ca4e961");
                //new Store.XML.FileService("A0.xml", PorjId, A0.Estimate.Repo, Message).Execute();

                A0.Disconnect();
				Marshal.ReleaseComObject(A0);
                A0 = null;
            }
        }

        /// <summary>
        /// Вывод объектов А0 на консоль
        /// </summary>
        public class ObjectToConsole : Objects.It
        {
            public ObjectToConsole(IA0EstimateRepo Repo) : base(Repo) { }

            /// <summary>
            /// Выполнение действий над объектом А0
            /// </summary>
            protected override void Do(IA0Object Obj, int Level)
            {
                Console.WriteLine("{0}{1}: {2} ({3}){4} [{5}]",
                    new string('\t', Level),
                    Obj.Kind,
                    Obj.ID.GUID.ToString(),
                    Obj.Mark,
                    Obj.Name,
                    Obj.BusinessStage());
            }

            /// <summary>
            /// Опускаться ли по структуре ниже
            /// </summary>
            protected override bool IsDown(IA0Object Obj)
            {
                return Obj.Kind < EA0ObjectKind.okLS;
            }
        }
    }
}

