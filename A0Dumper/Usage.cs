// $Date: 2020-03-20 14:04:17 +0300 (Пт, 20 мар 2020) $
// $Revision: 51 $
// $Author: agalkin $

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A0Dumper
{
    /// <summary>
    /// Использование приложения
    /// </summary>
    public class ArgHelp
    {
        public void Help()
        {
            var Args = Environment.GetCommandLineArgs();

            Console.WriteLine("Запуск приложения {0} --команда [аргументы]", Args[0]);

            Console.WriteLine("Каманды");
            Console.WriteLine("version - версия A0 API");
            Console.WriteLine("help - справка по использованию");
            Console.WriteLine("list - вывод сметных объектов");
            Console.WriteLine("xml - вывод проекта в xml");
            Console.WriteLine("  аргументы");
            Console.WriteLine("  GUID проекта");
			Console.WriteLine("nsi_bases - вывод БД НСИ");
		}
    }
}
