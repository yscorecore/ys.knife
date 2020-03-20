//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System.Collections.Generic;
//using YS.Knife.TestData;

//namespace YS.Knife
//{
//    [TestClass]
//    public class KnifeFilterTest
//    {
//        [TestMethod]
//        public void ShouldGetInstanceIfNoFilters()
//        {
//            var services = new ServiceCollection();
//            var filters = new Dictionary<string, string>
//            {
//            };
//            var configuration = new ConfigurationBuilder().AddInMemoryCollection(filters).Build();
//            var sp = services.RegisteKnifeServices(configuration).BuildServiceProvider();
//            var instance = sp.GetService<MyService1>();
//            Assert.IsNotNull(instance);
//        }
//        [TestMethod]
//        public void ShouldNotGetInstanceWhenFilterAssemblyByName()
//        {
//            var services = new ServiceCollection();
//            var filters = new Dictionary<string, string>
//            {
//                ["Knife:Ignores:Assemblies:0"] =this.GetType().Assembly.GetName().Name
//            };
//            var configuration = new ConfigurationBuilder().AddInMemoryCollection(filters).Build();
//            var sp = services.RegisteKnifeServices(configuration).BuildServiceProvider();
//            var instance = sp.GetService<MyService1>();
//            Assert.IsNull(instance);
//        }
//        [TestMethod]
//        public void ShouldNotGetInstanceWhenFilterAssemblyByWildcard()
//        {
//            var services = new ServiceCollection();
//            var filters = new Dictionary<string, string>
//            {
//                ["Knife:Ignores:Assemblies:0"] = "*"
//            };
//            var configuration = new ConfigurationBuilder().AddInMemoryCollection(filters).Build();
//            var sp = services.RegisteKnifeServices(configuration).BuildServiceProvider();
//            var instance = sp.GetService<MyService1>();
//            Assert.IsNull(instance);
//        }

//        [TestMethod]
//        public void ShouldNotGetInstanceWhenFilterTypeByName()
//        {
//            var services = new ServiceCollection();
//            var filters = new Dictionary<string, string>
//            {
//                ["Knife:Ignores:Types:0"] = typeof(MyService1).FullName
//            };
//            var configuration = new ConfigurationBuilder().AddInMemoryCollection(filters).Build();
//            var sp = services.RegisteKnifeServices(configuration).BuildServiceProvider();
//            var instance = sp.GetService<MyService1>();
//            Assert.IsNull(instance);
//        }
//        [TestMethod]
//        public void ShouldNotGetInstanceWhenFilterTypeByWildcard()
//        {
//            var services = new ServiceCollection();
//            var filters = new Dictionary<string, string>
//            {
//                ["Knife:Ignores:Types:0"] = "*.MyService?"
//            };
//            var configuration = new ConfigurationBuilder().AddInMemoryCollection(filters).Build();
//            var sp = services.RegisteKnifeServices(configuration).BuildServiceProvider();
//            var instance = sp.GetService<MyService1>();
//            Assert.IsNull(instance);
//        }
//    }
//}
