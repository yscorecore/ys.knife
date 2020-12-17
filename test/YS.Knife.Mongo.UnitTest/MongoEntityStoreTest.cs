using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.Data;
using YS.Knife.Hosting;
using YS.Knife.Mongo.UnitTest.Contents;

namespace YS.Knife.Mongo.UnitTest
{
    [TestClass]
    public class MongoEntityStoreTest : Knife.Hosting.KnifeHost
    {
        [InjectConfiguration("connectionstrings:cms")]
        private readonly string _ = TestEnvironment.MongoConnectionString;

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
        public void ShouldUpdateEntitySuccess()
        {
            var store = this.GetService<IEntityStore<Topic>>();
            var entity = new Topic
            {
                Title = "title",
                Content = "content",
                CreateTime = DateTimeOffset.Now,
                Summary = "summary"
            };
            store.Add(entity);
            Assert.IsNotNull(entity.Id);
            var newEntity = new Topic { Id = entity.Id, Title = "new title", Content = "new content" };
            store.Update(newEntity, nameof(Topic.Title), nameof(Topic.Content));
            var topicInDb = store.FindByKey(entity.Id);
            Assert.IsNotNull(topicInDb);
            Assert.AreEqual("new title", topicInDb.Title);
            Assert.AreEqual("new content", topicInDb.Content);
            Assert.AreEqual("summary", topicInDb.Summary);
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
