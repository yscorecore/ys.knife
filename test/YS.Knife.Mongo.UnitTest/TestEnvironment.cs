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
        public static uint MongoPort1 { get; private set; } = 27011;
        public static uint MongoPort2 { get; private set; } = 27012;
        public static uint MongoPort3 { get; private set; } = 27013;
        public static string MongoConnectionString { get => $"mongodb://root:{WebUtility.UrlEncode(MongoPassword)}@127.0.0.1:{MongoPort1}?replicaSet=rs"; }
        [AssemblyInitialize()]
        public static void Setup(TestContext t)
        {
            DockerCompose.OutputLine = t.WriteLine;
            MongoPort1 = Utility.GetAvailableTcpPort(27017);
            MongoPort2 = Utility.GetAvailableTcpPort(27117);
            MongoPort3 = Utility.GetAvailableTcpPort(27217);
            MongoPassword = Utility.NewPassword();
            var hostReportPort = Utility.GetAvailableTcpPort(8901);
            DockerCompose.Up(new Dictionary<string, object>
            {
                ["MONGO_PORT1"] = MongoPort1,
                ["MONGO_PORT2"] = MongoPort2,
                ["MONGO_PORT3"] = MongoPort3,
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
