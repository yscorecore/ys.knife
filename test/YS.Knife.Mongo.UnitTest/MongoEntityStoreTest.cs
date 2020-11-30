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
        [TestMethod]
        public void ShouldGetStoreInstanceFromDIContainer()
        {
            var store = this.GetService<IEntityStore<Topic>>();
            Assert.IsNotNull(store);
        }
    }
}
