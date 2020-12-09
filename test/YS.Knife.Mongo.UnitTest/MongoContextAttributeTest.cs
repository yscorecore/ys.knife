using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Collections.Generic;
using YS.Knife.Hosting;
using YS.Knife.Mongo.UnitTest.Contents;

namespace YS.Knife.Mongo.UnitTest
{
    [TestClass]
    public class MongoContextAttributeTest : KnifeHost
    {

        public MongoContextAttributeTest()
            : base(new Dictionary<string, object>
            {
                ["connectionStrings:user_db"] = TestEnvironment.MongoConnectionString,
                ["connectionStrings:book_db"] = TestEnvironment.MongoConnectionString
            })
        {

        }
        [DataTestMethod]
        [DataRow(typeof(UserContext))]
        [DataRow(typeof(BookContext))]
        public void ShouldGetContextType(Type contextType)
        {
            var context = GetService(contextType);
            var baseContexts = GetService<IEnumerable<MongoContext>>();
            Assert.IsNotNull(context);
            Assert.IsNotNull(baseContexts);
            Assert.IsTrue(baseContexts.Contains(context));
        }
        [TestMethod]
        public void ShouldGetUserStoreWhenRegisteEntityStore()
        {
            var userStore = GetService<IEntityStore<User>>();
            Assert.IsNotNull(userStore);
        }
        [TestMethod]
        public void ShouldNotGetBookStoreWhenNotRegisteEntityStore()
        {
            var bookStore = GetService<IEntityStore<Book>>();
            Assert.IsNull(bookStore);
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
        [MongoContext("book_db", RegisterEntityStore = false)]
        public class BookContext : MongoContext
        {
            public BookContext(IMongoDatabase database) : base(database)
            {
            }

            public IMongoCollection<Book> Users { get; set; }
        }
        #endregion

    }
}
