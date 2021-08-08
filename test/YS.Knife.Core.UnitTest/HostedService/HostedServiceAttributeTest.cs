using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;


namespace YS.Knife.HostedService.UnitTest
{

    public class HostedServiceAttributeTest
    {
        [Fact]
        public void ShouldRunWhenBackServiceDefinedHostedClassAttribute()
        {
            using var host = Utility.BuildHost();
            var appLiftTime = host.Services.GetRequiredService<IHostApplicationLifetime>();
            Task.Delay(200).ContinueWith(_ => { appLiftTime.StopApplication(); });
            host.Run();
            BackService.Triggered.Should().Be(true);
        }
    }
}
