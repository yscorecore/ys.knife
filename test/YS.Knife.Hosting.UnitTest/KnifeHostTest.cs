using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    }
}
