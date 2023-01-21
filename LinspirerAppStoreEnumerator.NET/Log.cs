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
        public enum LogLevel { Error, Warn ,Info }

        public static void WriteLog(LogLevel logLevel, string text)
        {
            Colorful.Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}][{logLevel}]{(logLevel == LogLevel.Error ? "" : " ")}: {text}"
                , logLevel == LogLevel.Info ? Color.White : logLevel == LogLevel.Warn? Color.Yellow: Color.Red);
        }
    }
}
