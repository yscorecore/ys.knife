using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using YS.Knife.Hosting;

namespace YS.Knife.HostedService.UnitTest
{
    [TestClass]
    public class HostedClassAttributeTest
    {
        [TestMethod]
        public void ShouldRunWhenBackServiceDefinedHostedClassAttribute()
        {
            using (var knifeHost = new KnifeHost())
            {
                var appLiftTime = knifeHost.GetRequiredService<IHostApplicationLifetime>();
                Task.Delay(200).ContinueWith(_ => { appLiftTime.StopApplication(); });
                knifeHost.Run();
                Assert.AreEqual(true, BackService.Triggered);
            }
        }
    }
}
