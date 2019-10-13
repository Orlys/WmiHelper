
namespace Orlys.WmiHelper.Dev
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Text;
    using System.Text.RegularExpressions;

    class Program
    {
        static void Main(string[] args)
        {
            var wmi = Wmi.Get(WmiSubject.Win32_OperatingSystem, Transpiler.ReadOnlyDictionary, timeout: TimeSpan.FromSeconds(3));

            foreach (var o in wmi)
            {
                foreach (var item in o)
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine();
            }
            Console.WriteLine("...");
            Console.ReadKey();
        }
    }
}
