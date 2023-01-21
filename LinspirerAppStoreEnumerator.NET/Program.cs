using Fclp;
using System;
using System.Security.Cryptography;
using static LinspirerAppStoreEnumerator.NET.CmdArgsProcessor;
using static System.Net.Mime.MediaTypeNames;

namespace LinspirerAppStoreEnumerator.NET
{


    public class App
    {
        static FluentCommandLineParser<ApplicationArguments> Args = new();

        static void EnumerateApp(Object AppID)
        {
            var id = 10093;//(int)AppID;
            var download = new Task( () =>
                {
                    var mac = "4a";
                    var email = "null";
                    var url = $"https://cloud.linspirer.com:883/download.php?email={email}&appid={id}&swdid={mac}&version={new Random(DateTime.Now.Millisecond).Next(1, 9000000)}";
                    var save = @".\"+id.ToString()+".apk";
                    var http = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    request.Headers.Add("Range", "bytes=0-0");
                    request.Headers.Add("user-agent", "okhttp/3.10.0");

                    Log.WriteLog(Log.LogLevel.Info, $"App {id} downloading...");

                    var response = http.Send(request);

                    if(response.StatusCode!=System.Net.HttpStatusCode.Redirect)
                    {
                        Log.WriteLog(Log.LogLevel.Info, $"App {id} do not exist!");
                        return;
                    }

                    request = new HttpRequestMessage(HttpMethod.Get, response.Headers.Location);
                    response = http.Send(request);

                    try
                    {
                        response.EnsureSuccessStatusCode();
                        using (var fs = File.Open(save, FileMode.OpenOrCreate))
                        {
                            using (var ms = response.Content.ReadAsStream())
                            {
                                ms.CopyTo(fs);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLog(Log.LogLevel.Error, $"App {id} download faild! Error message: {ex.Message}");
                        return;
                    }

                    Log.WriteLog(Log.LogLevel.Info, $"App {id} downloaded!");
                });

            Log.WriteLog(Log.LogLevel.Info, $"App {id} started.");

            if (Args.Object.IsSaveApk)
            {
                download.Start();
                download.Wait();
            }

            //Log.WriteLog(Log.LogLevel.Error, $"App ID: {id} occurred error!");


            Log.WriteLog(Log.LogLevel.Info, $"App ID: {id} done.");
            Thread.Sleep(500);
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