using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.Hosting;
using static Greeter;

namespace YS.Knife.Grpc.Client.UnitTest
{
    [TestClass]
    public class GrpcClientTest : YS.Knife.Hosting.KnifeHost
    {

        [InjectConfiguration("GrpcServices")]
        private readonly IDictionary<string, object> GrpcServices = new Dictionary<string, object>
        {
            ["BaseAddress"] = TestEnvironment.TestServerUrl
        };

        [TestMethod]
        public void ShouldGetGreeterClientInstance()
        {
            var greeterClient = this.GetService<GreeterClient>();
            Assert.IsNotNull(greeterClient);
        }

        [TestMethod]
        public void ShouldGetInjectInterfaceInstance()
        {
            var greeterClient = this.GetService<IGreeterService>();
            Assert.IsNotNull(greeterClient);
        }

        [TestMethod]
        public async Task ShouldInvokeSayHelloFromInjectInterface()
        {
            var greeterClient = this.GetService<IGreeterService>();
            var result = await greeterClient.SayHello("abc");
            Assert.AreEqual("Hello,abc", result);
        }

        [TestMethod]
        public async Task ShouldInvokeGetForecastFromInjectInterface()
        {
            var greeterClient = this.GetService<IGreeterService>();
            var result = await greeterClient.GetForecast();
            Assert.AreEqual(5, result.Count);
        }
    }
}
