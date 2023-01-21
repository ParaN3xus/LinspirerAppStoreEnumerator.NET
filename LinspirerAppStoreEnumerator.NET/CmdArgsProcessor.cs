using Fclp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace LinspirerAppStoreEnumerator.NET
{
    public class CmdArgsProcessor
    {
        public FluentCommandLineParser<ApplicationArguments> Args;

        public CmdArgsProcessor()
        {
            Args = new();
        }

        public void ProcessArgs()
        {
            Args.Setup(arg => arg.FromId)
                .As('f', "fromid")
                .Required();

            Args.Setup(arg => arg.ToId)
                .As('t', "toid")
                .Required();

            Args.Setup(arg => arg.NumThread)
                .As('n', "numthread")
                .Required();

            Args.Setup(arg => arg.IsSaveApk)
                .As('s', "save")
                .Required();

            Args.Setup(arg => arg.IsRecalled)
                .As('r', "recallled")
                .Required();

            Args.SetupHelp("?", "help")
                  .Callback(text =>
                  {
                      Console.Write(
                          "LinspirerAppStoreEnumerator is a tool to download and get info of apps on Linspirer App Store\n\n" +
                          "Usage: LinspirerAppStoreEnumerator [-option] [value] [-booloptions] ...\n\n" +
                          "Available options and its shortname:");
                      Console.WriteLine(text);
                      System.Environment.Exit(0);
                  });
        }

        public class ApplicationArguments
        {
            public int FromId { get; set; }
            public int ToId { get; set; }
            public int NumThread { get; set; }
            public bool IsSaveApk { get; set; }
            public bool IsRecalled { get; set; }
        }
    }
}
