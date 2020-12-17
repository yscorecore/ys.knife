using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace YS.Knife.HostedService.UnitTest
{
    [TestClass]
    public class HostedServiceAttributeTest
    {
        [TestMethod]
        public void ShouldRunWhenBackServiceDefinedHostedClassAttribute()
        {
            using var host = Utility.BuildHost();
            var appLiftTime = host.Services.GetRequiredService<IHostApplicationLifetime>();
            Task.Delay(200).ContinueWith(_ => { appLiftTime.StopApplication(); });
            host.Run();
            Assert.AreEqual(true, BackService.Triggered);
        }
    }
}
