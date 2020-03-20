using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using Moq;

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
                Assert.NotNull(innerHost);
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


        [Fact]
        public void ShouldGetInjectServiceByCodeWhenUseConfigureDelegate()
        {
            using (var knifeHost = new KnifeHost(new string[0], (builder, serviceCollection) =>
            {
                serviceCollection.AddSingleton<ITest, ATest>();
            }))
            {
                var test = ServiceProviderServiceExtensions.GetService<ITest>(knifeHost);
                Assert.IsType<ATest>(test);
            }
        }

        [Fact]
        public void ShouldRunTempStageServiceWhenRunStageTemp()
        {
            var tempStageService = Mock.Of<IStageService>(p => p.StageName == "temp");
            using (var knifeHost = new KnifeHost(new string[] { "Knife:Stage=temp" }, (builder, serviceCollection) =>
            {
                serviceCollection.AddSingleton<IStageService>(tempStageService);
            }))
            {
                knifeHost.Run();
            }
            Mock.Get(tempStageService).Verify(p => p.Run(default), Times.Once);
        }
        [Fact]
        public void ShouldRunDefaultWhenUseDefaultStageName()
        {
            var defaultStageService = Mock.Of<IStageService>(p => p.StageName == "default");
            using (var knifeHost = new KnifeHost(new string[] { "Stage=default" }, (builder, serviceCollection) =>
            {
                serviceCollection.AddSingleton<IStageService>(defaultStageService);
            }))
            {
                var appLiftTime = knifeHost.GetRequiredService<IHostApplicationLifetime>();
                Task.Delay(500).ContinueWith(_ => { appLiftTime.StopApplication(); });
                knifeHost.Run();
            }
            Mock.Get(defaultStageService).Verify(p => p.Run(default), Times.Never);
        }

        interface ITest { }
        class ATest : ITest { }

    }
}
