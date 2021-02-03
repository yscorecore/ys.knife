using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.Testing;

namespace YS.Knife.Grpc.Client.UnitTest
{
    [TestClass]
    public class TestEnvironment
    {
        public static string TestServerUrl { get; private set; } = "http://127.0.0.1:8080";
        [AssemblyInitialize]
        public static void Setup(TestContext t)
        {
            DockerCompose.OutputLine = t.WriteLine;
            var httpPort = Utility.GetAvailableTcpPort(8080);
            var httpsPort = Utility.GetAvailableTcpPort(8443);
            var hostReportPort = Utility.GetAvailableTcpPort(8901);
            StartContainer(httpPort, httpsPort, hostReportPort);

        }

        [AssemblyCleanup]
        public static void TearDown()
        {
            DockerCompose.Down();
        }
        private static void StartContainer(uint port, uint httpsPort, uint reportPort)
        {

            DockerCompose.Up(new Dictionary<string, object>
            {
                ["HTTP_PORT"] = port,
                ["HTTPS_PORT"] = httpsPort
            }, reportPort);
            TestServerUrl = $"http://127.0.0.1:{port}";
        }

        private static bool IsMacOs()
        {
            return Environment.OSVersion.Platform == PlatformID.MacOSX;
        }

    }
}
