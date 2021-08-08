using System;
using System.Collections.Generic;
using System.Net;
using Xunit;
using YS.Knife.Testing;

namespace YS.Knife.Mongo.UnitTest
{
    [CollectionDefinition(nameof(TestEnvironment))]
    public class TestEnvironment : IDisposable, ICollectionFixture<TestEnvironment>
    {
        public static string MongoPassword { get; private set; } = "example";
        public static uint MongoPort1 { get; private set; } = 27017;

        public static string MongoConnectionString { get => $"mongodb://root:{WebUtility.UrlEncode(MongoPassword)}@127.0.0.1:{MongoPort1}?replicaSet=rs"; }

        public TestEnvironment()
        {
            MongoPort1 = Utility.GetAvailableTcpPort(27017);
            MongoPassword = Utility.NewPassword();
            var hostReportPort = Utility.GetAvailableTcpPort(8901);
            DockerCompose.Up(new Dictionary<string, object>
            {
                ["MONGO_PORT1"] = MongoPort1,
                ["MONGO_PASSWORD"] = MongoPassword
            }, hostReportPort);
        }

        public void Dispose()
        {
            DockerCompose.Down();
        }
       

    }
}
