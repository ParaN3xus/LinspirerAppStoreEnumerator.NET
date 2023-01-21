using Fclp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                .SetDefault(true);

            Args.Setup(arg => arg.IsRecalled)
                .As('r', "recallled")
                .SetDefault(true);

            Args.SetupHelp("?", "help")
                  .Callback(text => Console.WriteLine(text));
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
