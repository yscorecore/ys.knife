using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.Test;

namespace YS.Knife.Mongo.UnitTest
{
    //[TestClass]
    public class TestEnvironment
    {
        public static string MongoPassword { get; private set; } = "example";
        public static uint MongoPort { get; private set; } = 27017;

        public static string MongoConnectionString { get => $"mongodb://root:{WebUtility.UrlEncode(MongoPassword)}@127.0.0.1:{MongoPort}"; }
        [AssemblyInitialize()]
        public static void Setup(TestContext t)
        {
            DockerCompose.OutputLine = t.WriteLine;
            MongoPort = Utility.GetAvailableTcpPort(27017);
            MongoPassword = Utility.NewPassword();
            var hostReportPort = Utility.GetAvailableTcpPort(8901);
            DockerCompose.Up(new Dictionary<string, object>
            {
                ["MONGO_PORT"] = MongoPort,
                ["MONGO_PASSWORD"] = MongoPassword
            }, hostReportPort);

        }

        [AssemblyCleanup()]
        public static void TearDown()
        {
            DockerCompose.Down();
        }

    }
}
