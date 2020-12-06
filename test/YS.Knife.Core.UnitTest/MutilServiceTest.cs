using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.TestData;

namespace YS.Knife
{
    [TestClass]
    public class MultiServiceTest
    {
        [TestMethod]
        public void ShouldGetMultiInstanceWhenMultiDefineKnifeAttribute()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();
            var sp = services.RegisterKnifeServices(configuration).BuildServiceProvider();
            var instances = sp.GetServices<MultiService>();
            Assert.AreEqual(4, instances.Count());
        }

        [TestMethod]
        public void ShouldNotGetInstanceWhenJustDefineKnifeAttributeInParents()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();
            var sp = services.RegisterKnifeServices(configuration).BuildServiceProvider();
            var instances = sp.GetServices<SubClass>();
            Assert.AreEqual(0, instances.Count());
        }

        [TestMethod]
        public void ShouldGetMultiInstanceWhenMultiDefineKnifeAttributeInNestedType()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();
            var sp = services.RegisterKnifeServices(configuration).BuildServiceProvider();
            var instances = sp.GetServices<OutterClass.InnerClass>();
            Assert.AreEqual(4, instances.Count());
        }

    }
}
