using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using YS.Knife.Data;
namespace YS.Knife.Mongo.UnitTest
{
    [TestClass]
    public class MongoContextTest : YS.Knife.Hosting.KnifeHost
    {
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
