using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Linq;
using YS.Knife.TestData;

namespace YS.Knife
{
    [TestClass]
    public class MutilServiceTest
    {
        [TestMethod]
        public void ShouldGetMutilInstanceWhenMutilDefineKnifeAttribute()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();
            var sp = services.RegisteKnifeServices(configuration).BuildServiceProvider();
            var instances = sp.GetServices<MutilSerivce>();
            Assert.AreEqual(4, instances.Count());
        }

        [TestMethod]
        public void ShouldGetMutilInstanceWhenMutilDefineKnifeAttributeInNestedType()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();
            var sp = services.RegisteKnifeServices(configuration).BuildServiceProvider();
            var instances = sp.GetServices<OutterClass.InnerClass>();
            Assert.AreEqual(4, instances.Count());
        }

    }
}