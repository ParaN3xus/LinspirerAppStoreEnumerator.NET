using Fclp;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace LinspirerAppStoreEnumerator.NET
{
    public class ApplicationArguments
    {
        public int FromId { get; set; }
        public int ToId { get; set; }
        public int NumThread { get; set; }
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

            p.Setup(arg => arg.FromId)
                .As('f', "fromid")
                .Required();

            p.Setup(arg => arg.ToId)
                .As('t', "toid")
                .Required();

            p.Setup(arg => arg.NumThread)
                .As('n', "numthread")
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

            ThreadPool.SetMaxThreads(args.Object.NumThread, args.Object.NumThread);

            for (var i = args.Object.FromId; i <= args.Object.ToId; i++)
            {
                ThreadPool.QueueUserWorkItem(EnumerateApp, i);
            }

            while (ThreadPool.CompletedWorkItemCount < args.Object.ToId-args.Object.FromId+1)
            {
                Thread.Sleep(100);
            }

            return 0;
        }
    } 



}