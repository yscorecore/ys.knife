using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace YS.Knife.Test
{
    public class DockerCompose
    {
        public static void Up(IDictionary<string, object> envs = null)
        {

            Exec("docker-compose", "up -d", envs);

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
                    startInfo.Environment.Add(kv.Key, Convert.ToString(kv.Value));
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
