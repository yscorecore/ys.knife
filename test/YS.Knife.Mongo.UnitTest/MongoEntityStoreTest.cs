using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.Data;
using YS.Knife.Mongo.UnitTest.Contents;

namespace YS.Knife.Mongo.UnitTest
{
    [TestClass]
    public class MongoEntityStoreTest : Knife.Hosting.KnifeHost
    {
        public MongoEntityStoreTest() : base(new Dictionary<string, object>
        {
            ["ConnectionStrings:cms"] = TestEnvironment.MongoConnectionString
        })
        {
        }

        [TestMethod]
        public void ShouldGetStoreInstanceFromDiContainer()
        {
            var store = this.GetService<IEntityStore<Topic>>();
            Assert.IsNotNull(store);
        }

        [TestMethod]
        public void ShouldAddEntitySuccess()
        {
            var store = this.GetService<IEntityStore<Topic>>();
            string title = $"Title_{SequentialKey.NewString()}";
            store.Add(new Topic
            {
                Title = title,
                Content = "This is content",
                CreateTime = DateTimeOffset.Now,
                Summary = "This is summary"
            });
            var topicInDb = store.Query(p => p.Title == title).FirstOrDefault();
            Assert.IsNotNull(topicInDb);
            Assert.AreEqual(title, topicInDb.Title);
        }
        
        [TestMethod]
        public void ShouldDeleteEntitySuccess()
        {
            var store = this.GetService<IEntityStore<Topic>>();
            var entity = new Topic
            {
                Title = "title",
                Content = "This is content",
                CreateTime = DateTimeOffset.Now,
                Summary = "This is summary"
            };
            store.Add(entity);
            Assert.IsNotNull(entity.Id);
            store.Delete(entity);
            var topicInDb = store.FindByKey(entity.Id);
            Assert.IsNull(topicInDb);
        }
    }
}
