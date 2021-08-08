using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using YS.Knife.Hosting;
using static Greeter;

namespace YS.Knife.Grpc.Client.UnitTest
{

    public class GrpcClientTest : YS.Knife.Hosting.KnifeHost
    {

        [InjectConfiguration("GrpcServices")]
        private readonly IDictionary<string, object> GrpcServices = new Dictionary<string, object>
        {
            ["BaseAddress"] = TestEnvironment.TestServerUrl
        };

        [Fact]
        public void ShouldGetGreeterClientInstance()
        {
            var greeterClient = this.GetService<GreeterClient>();
            greeterClient.Should().NotBeNull();
        }

        [Fact]
        public void ShouldGetInjectInterfaceInstance()
        {
            var greeterClient = this.GetService<IGreeterService>();
            greeterClient.Should().NotBeNull();
        }

        [Fact]
        public async Task ShouldInvokeSayHelloFromInjectInterface()
        {
            var greeterClient = this.GetService<IGreeterService>();
            var result = await greeterClient.SayHello("abc");
            result.Should().Be("Hello,abc");
        }

        [Fact]
        public async Task ShouldInvokeGetForecastFromInjectInterface()
        {
            var greeterClient = this.GetService<IGreeterService>();
            var result = await greeterClient.GetForecast();
            result.Count.Should().Be(5);
        }
    }
}
