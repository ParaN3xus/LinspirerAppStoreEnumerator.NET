using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinspirerAppStoreEnumerator.NET
{
    public static class Log
    {
        public enum LogLevel { Error, Info }

        public static void WriteLog(LogLevel logLevel, string text)
        {
            Colorful.Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}][{logLevel}]{(logLevel == LogLevel.Info ? " " : "")}: {text}"
                , logLevel == LogLevel.Info ? Color.White : Color.Red);
        }
    }
}
