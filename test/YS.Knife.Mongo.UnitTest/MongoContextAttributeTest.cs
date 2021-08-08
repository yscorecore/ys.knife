using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using MongoDB.Driver;
using YS.Knife.Hosting;
using System.Collections;

namespace YS.Knife.Mongo.UnitTest
{
    public class MongoContextAttributeTest : KnifeHost
    {
        [InjectConfiguration("connectionStrings:book_db")]
        private readonly string book_conn = TestEnvironment.MongoConnectionString;
        [InjectConfiguration("connectionStrings:user_db")]
        private readonly string user_conn = TestEnvironment.MongoConnectionString;

        [Theory]
        [InlineData(typeof(UserContext))]
        [InlineData(typeof(BookContext))]
        public void ShouldGetContextType(Type contextType)
        {
            var context = GetService(contextType);
            IEnumerable baseContexts = GetService<IEnumerable<MongoContext>>();
            context.Should().NotBeNull();
            baseContexts.Should().NotBeNull();
            baseContexts.Should().Contain(context);
        }
        [Fact]
        public void ShouldGetUserStoreWhenRegisteEntityStore()
        {
            var userStore = GetService<IEntityStore<User>>();
            userStore.Should().NotBeNull();
        }
        [Fact]
        public void ShouldGetBookStoreWhenRegisteEntityStore()
        {
            var userStore = GetService<IEntityStore<User>>();
            userStore.Should().NotBeNull();
        }
        [Fact]
        public void ShouldNotGetNewBookStoreWhenNotRegisteEntityStore()
        {
            var bookStore = GetService<IEntityStore<NewBook>>();
            bookStore.Should().BeNull();
        }
        #region UserContext

        public class User
        {
            public string Id { get; set; }
            public int Age { get; set; }
        }
        [MongoContext("user_db")]
        public class UserContext : MongoContext
        {
            public UserContext(IMongoDatabase database) : base(database)
            {
            }

            public IMongoCollection<User> Users { get; set; }
        }
        #endregion

        #region BookContext
        public class Book
        {
            public string Id { get; set; }
            public int Name { get; set; }
        }
        public class NewBook
        {
            public string Id { get; set; }
            public int Name { get; set; }
        }
        [MongoContext("book_db", RegisterEntityStore = false)]
        public class BookContext : MongoContext
        {
            public BookContext(IMongoDatabase database) : base(database)
            {
            }

            public IMongoCollection<Book> Books { get; set; }
        }
        #endregion

    }
}
