using Fclp;
using System;
using static LinspirerAppStoreEnumerator.NET.CmdArgsProcessor;
using static System.Net.Mime.MediaTypeNames;

namespace LinspirerAppStoreEnumerator.NET
{


    public class App
    {
        static FluentCommandLineParser<ApplicationArguments> Args = new();

        static void EnumerateApp(Object AppID)
        {
            var id = (int)AppID;

            Log.WriteLog(Log.LogLevel.Info, $"App ID: {id} started.");

            Log.WriteLog(Log.LogLevel.Error, $"App ID: {id} occurred error!");

            Log.WriteLog(Log.LogLevel.Info, $"App ID: {id} done.");

        }

        public static int Main(string[] RawArgs)
        {
            var argp = new CmdArgsProcessor();
            argp.ProcessArgs();
            Args = argp.Args;

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