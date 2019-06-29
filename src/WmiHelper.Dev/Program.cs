
namespace WmiHelper.Dev
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
            var wmi = Wmi.Get(WmiSubject.Win32_NetworkAdapter, Transpiler.ReadOnlyDictionary);
            foreach (var o in wmi)
            {
                foreach (var item in o)
                {
                    if(item.Value is object[] os)
                    {
                        Console.WriteLine(item.Key + ": "+ string.Join(", ", os));
                    }
                    else
                    Console.WriteLine(item);
                }
                Console.WriteLine();
            }
            Console.ReadKey();
        }
    }
}
