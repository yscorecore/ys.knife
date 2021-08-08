using System;
using System.Collections.Generic;
using Xunit;
using YS.Knife.Testing;

namespace YS.Knife.Rest.Client.UnitTest
{
    [CollectionDefinition(nameof(TestEnvironment))]
    public class TestEnvironment : IDisposable, ICollectionFixture<TestEnvironment>
    {
        public static string TestServerUrl { get; private set; } = "http://127.0.0.1:8080";

        public TestEnvironment()
        {
            var availablePort = Utility.GetAvailableTcpPort(8080);
            var hostReportPort = Utility.GetAvailableTcpPort(8901);
            StartContainer(availablePort, hostReportPort);
        }
        private static void StartContainer(uint port, uint reportPort)
        {
            DockerCompose.Up(new Dictionary<string, object>
            {
                ["SERVER_PORT"] = port
            }, reportPort);
            TestServerUrl = $"http://127.0.0.1:{port}";
        }

        public void Dispose()
        {
            DockerCompose.Down();
        }
    }
}
