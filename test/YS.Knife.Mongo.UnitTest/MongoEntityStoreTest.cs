using System;
using System.Linq;
using FluentAssertions;
using Xunit;
using YS.Knife.Data;
using YS.Knife.Hosting;
using YS.Knife.Mongo.UnitTest.Contents;

namespace YS.Knife.Mongo.UnitTest
{
    [Collection(nameof(TestEnvironment))]
    public class MongoEntityStoreTest : Knife.Hosting.KnifeHost
    {
        [InjectConfiguration("connectionstrings:cms")]
        private readonly string _ = TestEnvironment.MongoConnectionString;

        [Fact]
        public void ShouldGetStoreInstanceFromDiContainer()
        {
            var store = this.GetService<IEntityStore<Topic>>();
            store.Should().NotBeNull();
        }

        [Fact]
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
            topicInDb.Should().NotBeNull();
            topicInDb.Title.Should().Be(title);
        }
        [Fact]
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
            entity.Id.Should().NotBeNull();
            var newEntity = new Topic { Id = entity.Id, Title = "new title", Content = "new content" };
            store.Update(newEntity, nameof(Topic.Title), nameof(Topic.Content));
            var topicInDb = store.FindByKey(entity.Id);
            topicInDb.Should().NotBeNull();
            topicInDb.Title.Should().Be("new title");
            topicInDb.Content.Should().Be("new content");
            topicInDb.Summary.Should().Be("summary");
        }

        [Fact]
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
            entity.Id.Should().NotBeNull();
            store.Delete(entity);
            var topicInDb = store.FindByKey(entity.Id);
            topicInDb.Should().BeNull();
        }
    }
}
