using Amib.Threading;
using Fclp;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using static LinspirerAppStoreEnumerator.NET.CmdArgsProcessor;

namespace LinspirerAppStoreEnumerator.NET
{
    public class App
    {
        static FluentCommandLineParser<ApplicationArguments> Args = new();

        static object EnumerateApp(Object AppID)
        {
            var getinfosuccess = false;
            var id = (int)AppID;
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
            var getinfo1 = new Task(() =>
            {
                string packagename = "", targetapi = "", name = "", versionname = "", versioncode = "", md5sum = "", sha1 = "";

                var mac = "b4:cd:27:30:3e:f2";
                var username = "sxceshi1";
                var client = new HttpClient();
                var json = $"{{\"is_encrypt\": false, \"method\": \"com.linspirer.app.getappbyids\", \"id\": \"1\", \"!version\": \"1\", \"jsonrpc\": \"2.0\", \"params\": {{\"swdid\": \"{mac}\", \"username\": \"{username}\", \"token\": \"null\", \"ids\": [\"{id}\"]}}, \"client_version\": \"5.1.0\", \"_elapsed\": 1}}";
                var content = new StringContent(json);

                Log.WriteLog(Log.LogLevel.Info, $"App {id} getting info with YoungToday solution...");

                var response = client.PostAsync("https://cloud.linspirer.com:883/public-interface.php", content).Result;
                var data = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);

                try
                {
                    if (data == null)
                    {
                        Log.WriteLog(Log.LogLevel.Warn, $"App {id} getting info with YoungToday solution faild with blank response!");
                        return;
                    }
                    var datab = data.data;

                    if (data.code != 0)
                    {
                        Log.WriteLog(Log.LogLevel.Warn, $"App {id} getting info with YoungToday solution faild with invalid accound or swdid!");
                        return;
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
                    return;
                }

                using (StreamWriter sw = new StreamWriter("appinfo.csv", true, Encoding.UTF8))
                {
                    sw.WriteLine($"{id},{packagename},{targetapi},{name},{versionname},{versioncode},{md5sum},{sha1}");
                }

                getinfosuccess = true;
                Log.WriteLog(Log.LogLevel.Info, $"App {id} getting info success! {packagename},{targetapi},{name},{versionname},{versioncode},{md5sum},{sha1}");
            });
            var getinfo2 = new Task(() =>
            {
                string packagename = "", targetapi = "", name = "", versionname = "", versioncode = "", md5sum = "", sha1 = "";

                Process process = new();
                process.StartInfo.FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "aapt2-linux" : RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "aapt2-win.exe" : "aapt2-osx";
                process.StartInfo.Arguments = $"dump badging ./apks/{id}.apk";
                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.StandardOutputEncoding = Encoding.UTF8;

                Log.WriteLog(Log.LogLevel.Info, $"App {id} getting info with aapt2 solution...");

                process.Start();

                List<string> lines = new();
                var sr = process.StandardOutput;

                if (sr == null)
                {
                    Log.WriteLog(Log.LogLevel.Warn, $"App {id} getting info with aapt2 solution faild with no output!");
                    return;
                }

                while (!sr.EndOfStream)
                {
                    var text = sr.ReadLine();
                    if (text != null)
                    {
                        lines.Add(text);
                    }
                }

                process.WaitForExit();

                if (!File.Exists($"./apks/{id}.apk"))
                {
                    Log.WriteLog(Log.LogLevel.Warn, $"App {id} getting info with aapt2 solution faild with no apk file!");
                    return;
                }

                foreach (string line in lines)
                {
                    if (line.StartsWith(@"package: name='"))
                    {
                        var list = line.Split('\'');
                        packagename = list[1];
                        versioncode = list[3];
                        versionname = list[5];
                    }
                    else if (line.StartsWith(@"targetSdkVersion:'"))
                    {
                        var list = line.Split('\'');
                        targetapi = list[1];
                    }
                    else if (line.StartsWith(@"application-label:'"))
                    {
                        var list = line.Split('\'');
                        name = list[1];
                    }
                }

                using (var md5 = MD5.Create())
                {
                    using (var hash = SHA1.Create())
                    {
                        using (var stream = File.OpenRead($"./apks/{id}.apk"))
                        {
                            md5sum = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "");
                            sha1 = BitConverter.ToString(hash.ComputeHash(stream)).Replace("-", "");
                        }
                    }
                }

                Log.WriteLog(Log.LogLevel.Info, $"App {id} getting info success! {packagename},{targetapi},{name},{versionname},{versioncode},{md5sum},{sha1}");
                
                using (StreamWriter sw = new StreamWriter("appinfo.csv", true, Encoding.UTF8))
                {
                    sw.WriteLine($"{id},{packagename},{targetapi},{name},{versionname},{versioncode},{md5sum},{sha1}");
                }
            });

            Log.WriteLog(Log.LogLevel.Info, $"App {id} started.");

            if (Args.Object.IsSaveApk)
            {
                download.Start();
            }
            getinfo1.Start();
            getinfo1.Wait();

            if (!getinfosuccess && Args.Object.IsRecalled)
            {
                if (Args.Object.IsSaveApk)
                {
                    download.Wait();
                }
                getinfo2.Start();
                getinfo2.Wait();
            }

            download.Wait();
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
                Console.WriteLine("Wrong arguments. Run again with -? for help.");
                return 1;
            }

            var pool = new SmartThreadPool();

            pool.MaxQueueLength = Args.Object.NumThread;

            if (!Directory.Exists("./apks"))
            {
                Directory.CreateDirectory("./apks");
            }
            using (StreamWriter sw = new StreamWriter("appinfo.csv", false, Encoding.UTF8))
            {
                sw.WriteLine("ID,PackageName,TargetAPI,Name,VersionName,VersionCode,MD5,SHA1");
            }

            for (var i = Args.Object.FromId; i <= Args.Object.ToId; i++)
            {
                pool.QueueWorkItem(callback: EnumerateApp, state: i);
            }

            pool.WaitForIdle();

            Log.WriteLog(Log.LogLevel.Info, $"Done!");

            return 0;
        }
    }


}