using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;
namespace YS.Knife.Hosting
{
    
    public class KnifeHostTest
    {
        [Fact]
        public void ShouldCreateANewInstanceWhenCallCtor()
        {
            using (var host = new KnifeHost())
            {
            }
        }
        [Fact]
        public void ShouldReturnServiceWhenGetService()
        {
            using (var knifeHost = new KnifeHost(new string[0]))
            {
                var innerHost = knifeHost.GetRequiredService<IHost>();
                innerHost.Should().NotBeNull();
            }
        }
        [Fact]
        public void ShouldCanBeStopedWhenRun()
        {
            using (var knifeHost = new KnifeHost(new string[0]))
            {
                var appLiftTime = knifeHost.GetRequiredService<IHostApplicationLifetime>();
                Task.Delay(500).ContinueWith(_ => { appLiftTime.StopApplication(); });
                knifeHost.Run();
            }
        }
    }
}
