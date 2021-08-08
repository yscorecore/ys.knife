using System;
using System.Collections.Generic;
using Xunit;
using YS.Knife.Testing;

namespace YS.Knife.Grpc.Client.UnitTest
{
    
    public class TestEnvironment
    {
        public static string TestServerUrl { get; private set; } = "http://127.0.0.1:8080";
       //[AssemblyInitialize]
       // public static void Setup(TestContext t)
       // {
       //     DockerCompose.OutputLine = t.WriteLine;
       //     var httpPort = Utility.GetAvailableTcpPort(8080);
       //     var hostReportPort = Utility.GetAvailableTcpPort(8901);
       //     StartContainer(httpPort, hostReportPort);

       // }

       // [AssemblyCleanup]
       // public static void TearDown()
       // {
       //     DockerCompose.Down();
       // }
        private static void StartContainer(uint port, uint reportPort)
        {

            DockerCompose.Up(new Dictionary<string, object>
            {
                ["HTTP_PORT"] = port
            }, reportPort);
            TestServerUrl = $"http://127.0.0.1:{port}";
        }



    }
}
