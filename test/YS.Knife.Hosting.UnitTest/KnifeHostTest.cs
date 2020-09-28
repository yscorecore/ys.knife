using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YS.Knife.Stage;
namespace YS.Knife.Hosting
{
    [TestClass]
    public class KnifeHostTest
    {
        [TestMethod]
        public void ShouldCreateANewInstanceWhenCallCtor()
        {
            using (var host = new KnifeHost())
            {

            }

        }
        [TestMethod]
        public void ShouldReturnServiceWhenGetService()
        {
            using (var knifeHost = new KnifeHost(new string[0]))
            {
                var innerHost = knifeHost.GetRequiredService<IHost>();
                Assert.IsNotNull(innerHost);
            }
        }
        [TestMethod]
        public void ShouldCanBeStopedWhenRun()
        {
            using (var knifeHost = new KnifeHost(new string[0]))
            {
                var appLiftTime = knifeHost.GetRequiredService<IHostApplicationLifetime>();
                Task.Delay(500).ContinueWith(_ => { appLiftTime.StopApplication(); });
                knifeHost.Run();
            }
        }


        [TestMethod]
        public void ShouldGetInjectServiceByCodeWhenUseConfigureDelegate()
        {
            using (var knifeHost = new KnifeHost(new string[0], (builder, serviceCollection) =>
            {
                serviceCollection.AddSingleton<ITest, ATest>();
            }))
            {
                var test = ServiceProviderServiceExtensions.GetService<ITest>(knifeHost);
                Assert.IsTrue(test is ATest);
            }
        }

        [TestMethod]
        public void ShouldRunTempStageServiceWhenRunStageTemp()
        {
            var tempStageService = Mock.Of<IStageService>(p => p.StageName == "temp" && p.EnvironmentName == "*");
            using (var knifeHost = new KnifeHost(new string[] { "Knife:Stage=temp" }, (builder, serviceCollection) =>
            {
                serviceCollection.AddSingleton<IStageService>(tempStageService);
            }))
            {
                knifeHost.Run();
            }
            Mock.Get(tempStageService).Verify(p => p.Run(default), Times.Once);
        }

        [TestMethod]
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
