using Fclp;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace LinspirerAppStoreEnumerator.NET
{
    public class ApplicationArguments
    {
        public int StartId { get; set; }
        public int EndId { get; set; }
        public int ThreadNum { get; set; }
        public bool IsSaveApk { get; set; }
        public bool IsRecalled { get; set; }
    }

    public class App
    {
        static FluentCommandLineParser<ApplicationArguments> InitArgs()
        {
            var p = new FluentCommandLineParser<ApplicationArguments>();

            p.SetupHelp("?", "help")
                .Callback(()=>Console.WriteLine("ehh"));

            p.Setup(arg => arg.StartId)
                .As('s', "startid")
                .Required();

            p.Setup(arg => arg.StartId)
                .As('e', "endid")
                .Required();

            p.Setup(arg => arg.ThreadNum)
                .As('t', "thread")
                .Required();

            p.Setup(arg => arg.IsSaveApk)
                .As('s', "save")
                .SetDefault(true);

            p.Setup(arg => arg.IsRecalled)
                .As('r', "recallled")
                .SetDefault(true);

            return p;
        }

        static void EnumerateApp(Object stateInfo)
        {
            Console.WriteLine((int)stateInfo);
        }

        public static int Main(string[] RawArgs)
        {
            var args = InitArgs();

            if(args.Parse(RawArgs).HasErrors)
            {
                return 1;
            }

            ThreadPool.SetMaxThreads(args.Object.ThreadNum, args.Object.ThreadNum);

            for (var i = args.Object.StartId; i <= args.Object.EndId; i++)
            {
                ThreadPool.QueueUserWorkItem(EnumerateApp, i);
            }

            return 0;
        }
    } 



}