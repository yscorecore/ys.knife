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
        private IDisposable dockerCompose;
        public TestEnvironment()
        {
            var availablePort = Utility.GetAvailableTcpPort(8080);
            dockerCompose = DockerCompose.Up(new Dictionary<string, object>
            {
                ["SERVER_PORT"] = availablePort
            }, "testserver:80");
            TestServerUrl = $"http://127.0.0.1:{availablePort}";
        }


        public void Dispose()
        {
            dockerCompose?.Dispose();
        }
    }
}
