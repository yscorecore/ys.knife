using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

namespace YS.Knife.Test
{
    public static class DockerCompose
    {
        public static void Up(IDictionary<string, object> envs = null, bool waitStatusReady = true, int maxWaitStatusSeconds = 120)
        {
            envs = envs ?? new Dictionary<string, object>();

            string statusFolder = "tmp";
            string statusFileName = DateTimeOffset.Now.Ticks.ToString();
            string statusFile = System.IO.Path.Combine(statusFolder, statusFileName);

            envs.Add("STATUS_FILE", statusFileName);
            Exec("docker-compose", "up --build -d", envs);
            if (waitStatusReady)
            {
                for (int i = 0; i < maxWaitStatusSeconds; i++)
                {
                    if (System.IO.File.Exists(statusFile))
                    {
                        break;
                    }
                    Task.Delay(1000).Wait();
                }
            }
        }
        public static void Down()
        {
            Exec("docker-compose", "down", null);
        }
        private static int Exec(string fileName, string arguments, IDictionary<string, object> envs)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
            };
            if (envs != null)
            {
                foreach (var kv in envs)
                {
                    startInfo.Environment.Add(kv.Key, Convert.ToString(kv.Value, CultureInfo.InvariantCulture));
                }
            }
            var process = Process.Start(startInfo);
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                throw new Exception($"Exec process return {process.ExitCode}.");
            }
            return process.ExitCode;
        }
    }
}
