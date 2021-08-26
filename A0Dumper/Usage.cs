// $Date: 2019-10-15 14:18:33 +0300 (Вт, 15 окт 2019) $
// $Revision: 7 $
// $Author: vbutov $

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
