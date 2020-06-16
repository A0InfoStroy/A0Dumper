// $Date: 2020-03-20 14:04:17 +0300 (Пт, 20 мар 2020) $
// $Revision: 51 $
// $Author: agalkin $

using System;

namespace A0Dumper
{
    using A0Service;

    /// <summary>
    /// Вывод информации о модуле A0 API
    /// </summary>
    class AppVersion
    {
        public void Info(API A0)
        {
            Console.WriteLine("EditionDate {0}", A0.App.Version.EditionDate);
            Console.WriteLine("ProductVersion {0}", A0.App.Version.ProductVersion);
            Console.WriteLine("LicenseID {0}", A0.App.Version.LicenseID);
            Console.WriteLine("DBVersion {0}", A0.App.Version.DBVersion);
            Console.WriteLine("APIVersion {0}", A0.App.Version.Version);
            Console.WriteLine("");
            Console.WriteLine("ProcessID {0}", A0.App.ProcessID);
            Console.WriteLine("CurrentDir {0}", A0.App.CurrentDir);
            Console.WriteLine("");
        }
    }
}
