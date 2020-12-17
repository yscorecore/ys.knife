using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.Test;

namespace YS.Knife.Rest.Client.UnitTest
{
    [TestClass]
    public class TestEnvironment
    {
        public static string TestServerUrl { get; private set; } = "http://127.0.0.1:8080";
        [AssemblyInitialize()]
        public static void Setup(TestContext t)
        {
            DockerCompose.OutputLine = t.WriteLine;
            var availablePort = Utility.GetAvailableTcpPort(8080);
            var hostReportPort = Utility.GetAvailableTcpPort(8901);
            StartContainer(availablePort, hostReportPort);

        }

        [AssemblyCleanup()]
        public static void TearDown()
        {
            DockerCompose.Down();
        }
        private static void StartContainer(uint port, uint reportPort)
        {
            DockerCompose.Up(new Dictionary<string, object>
            {
                ["SERVER_PORT"] = port
            }, reportPort);
            TestServerUrl = $"http://127.0.0.1:{port}";
        }


    }
}
