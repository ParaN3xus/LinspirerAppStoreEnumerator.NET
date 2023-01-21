using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinspirerAppStoreEnumerator.NET
{
    public class Log
    {
        public enum LogLevel { Error, Info }

        public void WriteLog(LogLevel logLevel, string text)
        {
            if (logLevel == LogLevel.Error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine($"[{DateTime.Now.ToString("T")}][{logLevel}]:{text}");
        }
    }
}
