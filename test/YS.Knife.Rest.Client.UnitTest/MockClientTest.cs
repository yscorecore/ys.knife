using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.Hosting;
using YS.Knife.Rest.Client.UnitTest.Clients;

namespace YS.Knife.Rest.Client.UnitTest
{
    [TestClass]
    public class MockClientTest : KnifeHost
    {
        [TestMethod]
        public async Task MyTestMethod()
        {
            var mockClient = this.GetService<MockClient>();
            var value = await mockClient.GetValue();
            Assert.AreEqual(1, value);
        }
        [TestMethod]
        public async Task MyTestMethod2()
        {
            var mockClient = this.GetService<MockClient>();
            var value = await mockClient.GetObject();
            Assert.AreEqual("001", value.Id);
        }
    }
}
