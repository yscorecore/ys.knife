using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;

namespace YS.Knife.Test
{
    public static class DockerCompose
    {
        public static void Up(IDictionary<string, object> envs = null, int reportStatusPort = 8901, int maxWaitStatusSeconds = 120)
        {
            envs = envs ?? new Dictionary<string, object>();
            if (reportStatusPort > 0)
            {
                envs.Add("REPORT_TO_HOST_PORT", reportStatusPort);
            }
            Exec("docker-compose", "up --build -d", envs);
            if (reportStatusPort > 0)
            {
                WaitContainerReportStatus(reportStatusPort, maxWaitStatusSeconds);
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

        private static void WaitContainerReportStatus(int port = 8901, int maxSeconds = 120)
        {
            using (var httpListener = new HttpListener())
            {
                httpListener.Prefixes.Add($"http://+:{port}/");
                httpListener.Start();
                IAsyncResult result = httpListener.BeginGetContext(new AsyncCallback(ListenerCallback), httpListener);
                Console.WriteLine("Waiting for request to be processed asyncronously.");
                result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(maxSeconds));
                Console.WriteLine("Request processed asyncronously.");
            }
        }
        private static void ListenerCallback(IAsyncResult result)
        {
            HttpListener listener = (HttpListener)result.AsyncState;
            // Call EndGetContext to complete the asynchronous operation.
            HttpListenerContext context = listener.EndGetContext(result);
            // Obtain a response object.
            HttpListenerResponse response = context.Response;
            response.StatusCode = (int)HttpStatusCode.OK;
        }
    }
}
