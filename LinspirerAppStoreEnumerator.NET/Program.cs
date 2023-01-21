using Amib.Threading;
using Fclp;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Xml.Linq;
using static LinspirerAppStoreEnumerator.NET.CmdArgsProcessor;
using static System.Net.Mime.MediaTypeNames;

namespace LinspirerAppStoreEnumerator.NET
{


    public class App
    {
        static FluentCommandLineParser<ApplicationArguments> Args = new();

        static object EnumerateApp(Object AppID)
        {
            var id = (int)AppID;//(int)AppID;
            var download = new Task(() =>
                {
                    var mac = "4a";
                    var email = "null";
                    var url = $"https://cloud.linspirer.com:883/download.php?email={email}&appid={id}&swdid={mac}&version={new Random(DateTime.Now.Millisecond).Next(1, 9000000)}";
                    var save = @"./apks/" + id.ToString() + ".apk";
                    var http = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    request.Headers.Add("Range", "bytes=0-0");
                    request.Headers.Add("user-agent", "okhttp/3.10.0");

                    Log.WriteLog(Log.LogLevel.Info, $"App {id} downloading...");

                    var response = http.Send(request);

                    if (response.StatusCode != System.Net.HttpStatusCode.Redirect)
                    {
                        Log.WriteLog(Log.LogLevel.Warn, $"App {id} do not exist!");
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
            var getinfo = new Task(() =>
            {
                string packagename, targetapi, name, versionname, versioncode, md5sum, sha1;

                var mac = "4a";
                var username = "sxceshi1";
                var client = new HttpClient();
                var json = $"{{\"is_encrypt\": false, \"method\": \"com.linspirer.app.getappbyids\", \"id\": \"1\", \"!version\": \"1\", \"jsonrpc\": \"2.0\", \"params\": {{\"swdid\": {mac}, \"username\": {username}, \"token\": \"null\", \"ids\": [\"<built-in function id>\"]}, \"client_version\": \"5.1.0\", \"_elapsed\": 1}";
                var content = new StringContent(json);

                Log.WriteLog(Log.LogLevel.Info, $"App {id} getting info with YoungToday solution...");

                var response = client.PostAsync("https://cloud.linspirer.com:883/public-interface.php", content).Result;
                var data = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);

                try
                {
                    var datab = data.data;

                    if (data.code != 0)
                    {
                        Log.WriteLog(Log.LogLevel.Warn, $"App {id} getting info with YoungToday solution faild with invalid accound or swdid!");
                    }

                    packagename = datab[0].packagename;
                    targetapi = datab[0].target_sdk_version;
                    name = datab[0].name;
                    versionname = datab[0].versionname;
                    versioncode = datab[0].versioncode;
                    md5sum = datab[0].md5sum;
                    sha1 = datab[0].sha1 != null ? datab[0].sha1 : "no sha1";
                }
                catch (Exception ex)
                {
                    Log.WriteLog(Log.LogLevel.Warn, $"App {id} getting info with YoungToday solution faild with {ex.Message}");
                    // solution2

                }
            });

            Log.WriteLog(Log.LogLevel.Info, $"App {id} started.");

            if (Args.Object.IsSaveApk)
            {
                download.Start();
            }




            download.Wait();
            //Log.WriteLog(Log.LogLevel.Error, $"App ID: {id} occurred error!");

            Log.WriteLog(Log.LogLevel.Info, $"App ID: {id} done.");
            return 0;
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

            var pool = new SmartThreadPool();

            pool.MaxQueueLength=Args.Object.NumThread;

            for (var i = Args.Object.FromId; i <= Args.Object.ToId; i++)
            {
                pool.QueueWorkItem(callback: EnumerateApp, state: i);
            }
                
            pool.WaitForIdle();

            return 0;
        }
    }


}