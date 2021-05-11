using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace YS.Knife.Testing
{
    public static class DockerCompose
    {
        public static Action<string> OutputLine { get; set; } = Console.WriteLine;
        public static int MaxTimeOutSeconds { get; set; } = 60 * 30;

        public static void Up(IDictionary<string, object> envs = null)
        {
            envs ??= new Dictionary<string, object>();
            Exec("docker-compose", "up --build -d", envs, OutputLine ?? Console.WriteLine);
        }

        public static void Up(IDictionary<string, object> envs, uint reportStatusPort, int maxWaitStatusSeconds = 120)
        {
            envs ??= new Dictionary<string, object>();





            RunDockerComposeAndWaitContainerReportStatus(envs, reportStatusPort,
             maxWaitStatusSeconds);


        }
        public static void Down()
        {
            Exec("docker-compose", "down", null, OutputLine ?? Console.WriteLine);
        }
        private static int Exec(string fileName, string arguments, IDictionary<string, object> envs, Action<string> outputLine)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardError = true,
                RedirectStandardOutput = true,

            };
            if (envs != null)
            {
                foreach (var kv in envs)
                {
                    startInfo.Environment.Add(kv.Key, Convert.ToString(kv.Value, CultureInfo.InvariantCulture));
                }
            }
            using (var process = Process.Start(startInfo))
            {

                using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
                {
                    process.OutputDataReceived += (s, e) =>
                    {
                        if (e.Data == null)
                        {
                            outputWaitHandle.Set();
                        }
                        else
                        {
                            outputLine(e.Data);
                        }
                    };
                    process.ErrorDataReceived += (s, e) =>
                    {
                        if (e.Data == null)
                        {
                            errorWaitHandle.Set();
                        }
                        else
                        {
                            outputLine(e.Data);
                        }
                    };
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    var timeout = MaxTimeOutSeconds * 1000;
                    if (process.WaitForExit(timeout) && outputWaitHandle.WaitOne(timeout) && errorWaitHandle.WaitOne(timeout))
                    {
                        if (process.ExitCode != 0)
                        {
                            throw new Exception($"Exec process return {process.ExitCode}.");
                        }
                    }
                    else
                    {
                        throw new Exception($"Exec process timeout, total seconds > {MaxTimeOutSeconds}s.");
                    }
                }
                return process.ExitCode;
            }

        }

        private static void RunDockerComposeAndWaitContainerReportStatus(IDictionary<string, object> envs, uint port = 8901, int maxSeconds = 120)
        {
            using (var httpListener = new HttpListener())
            {
                httpListener.Prefixes.Add($"http://+:{port}/");
                httpListener.Start();
                IAsyncResult result = httpListener.BeginGetContext(new AsyncCallback(ListenerCallback), httpListener);
                Console.WriteLine("Waiting for request to be processed asyncronously.");
                Task.Run(() =>
                {
                    envs.Add("REPORT_TO_HOST_PORT", port);
                    Exec("docker-compose", "up --build -d", envs, OutputLine ?? Console.WriteLine);
                });

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
