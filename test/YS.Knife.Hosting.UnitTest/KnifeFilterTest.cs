using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.Hosting;
using YS.Knife.TestData;

namespace YS.Knife
{
    [TestClass]
    public class KnifeFilterTest
    {
        [TestMethod]
        public void ShouldGetInstanceIfNoFilters()
        {
            using (var host = new KnifeHost(
                new Dictionary<string, object>
                {
                }))
            {
                var instance = host.GetService<MyService1>();
                Assert.IsNotNull(instance);
            }

        }
        [TestMethod]
        public void ShouldNotGetInstanceWhenFilterAssemblyByName()
        {
            using (var host = new KnifeHost(
               new Dictionary<string, object>
               {
                   ["Knife:Ignores:Assemblies:0"] = this.GetType().Assembly.GetName().Name
               }))
            {
                var instance = host.GetService<MyService1>();
                Assert.IsNull(instance);
            }
        }
        [TestMethod]
        public void ShouldNotGetInstanceWhenFilterAssemblyByWildcard()
        {
            using (var host = new KnifeHost(
              new Dictionary<string, object>
              {
                  ["Knife:Ignores:Assemblies:0"] = "*"
              }))
            {
                var instance = host.GetService<MyService1>();
                Assert.IsNull(instance);
            }
        }

        [TestMethod]
        public void ShouldNotGetInstanceWhenFilterTypeByName()
        {
            using (var host = new KnifeHost(
              new Dictionary<string, object>
              {
                  ["Knife:Ignores:Types:0"] = typeof(MyService1).FullName
              }))
            {
                var instance = host.GetService<MyService1>();
                Assert.IsNull(instance);
            }
        }
        [TestMethod]
        public void ShouldNotGetInstanceWhenFilterTypeByWildcard()
        {
            using (var host = new KnifeHost(
             new Dictionary<string, object>
             {
                 ["Knife:Ignores:Types:0"] = "*.MyService?"
             }))
            {
                var instance = host.GetService<MyService1>();
                Assert.IsNull(instance);
            }
        }
    }
}
