using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using YS.Knife.Hosting;

namespace YS.Knife.Mongo.UnitTest
{
    [TestClass]
    public class MongoContextTest : KnifeHost
    {
        [InjectConfiguration("connectionStrings:cms")]
        private readonly string _ = TestEnvironment.MongoConnectionString;

        [TestMethod]
        public void ShouldGetContextInstanceFromDIContainer()
        {
            var context = this.GetService<Contents.ContentManageContext>();
            Assert.IsNotNull(context);
        }

        [TestMethod]
        public void ShouldInsertTopicItemSuccess()
        {
            var context = this.GetService<Contents.ContentManageContext>();
            context.Topic.InsertOne(new Contents.Topic
            {
                Content = "this is content",
                Title = "first topic",
                Summary = "this is summary",
                CreateTime = DateTimeOffset.Now,
                Statistics = new Contents.TopicStatistic
                {
                    Liked = 10,
                    Viewed = 100,
                },
            });
            Assert.IsNotNull(context);
        }

        [TestMethod]
        public void ShouldQueryListTopicItemSuccess()
        {
            var context = this.GetService<Contents.ContentManageContext>();
            var topics = context.Topic.AsQueryable().Where(p => p.CreateTime < DateTimeOffset.Now).ToList();
            Assert.IsNotNull(topics);
        }
    }
}
