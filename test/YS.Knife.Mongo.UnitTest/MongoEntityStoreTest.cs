using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.Mongo.UnitTest.Contents;

namespace YS.Knife.Mongo.UnitTest
{
    [TestClass]
    public class MongoEntityStoreTest : Knife.Hosting.KnifeHost
    {
        public MongoEntityStoreTest() : base(new Dictionary<string, object>
        {
            ["ConnectionStrings__cms"] = TestEnvironment.MongoConnectionString
        })
        {
        }
        [TestMethod]
        public void ShouldGetStoreInstanceFromDIContainer()
        {
            var store = this.GetService<IEntityStore<Topic>>();
            Assert.IsNotNull(store);
        }
    }
}
