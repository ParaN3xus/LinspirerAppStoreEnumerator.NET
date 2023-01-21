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
        static FluentCommandLineParser<ApplicationArguments> Args = new();

        static void InitArgs()
        {
            Args.SetupHelp("?", "help")
                .Callback(() => Console.WriteLine("ehh"));

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
        }

        static void EnumerateApp(Object AppID)
        {
            var id = (int)AppID;
        }

        public static int Main(string[] RawArgs)
        {
            if (Args.Parse(RawArgs).HasErrors)
            {
                return 1;
            }

            ThreadPool.SetMaxThreads(Args.Object.NumThread, Args.Object.NumThread);

            for (var i = Args.Object.FromId; i <= Args.Object.ToId; i++)
            {
                ThreadPool.QueueUserWorkItem(EnumerateApp, i);
            }

            while (ThreadPool.CompletedWorkItemCount < Args.Object.ToId - Args.Object.FromId + 1)
            {
                Thread.Sleep(100);
            }

            return 0;
        }
    }


}