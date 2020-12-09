using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using YS.Knife.Hosting;
using YS.Knife.Entity;
namespace YS.Knife.Mongo.UnitTest
{

    [TestClass]
    public class TransactionAttributeTest : KnifeHost
    {
        public TransactionAttributeTest()
            : base(new Dictionary<string, object>
            {
                ["connectionStrings:user_db"] = TestEnvironment.MongoConnectionString,
                ["connectionStrings:book_db"] = TestEnvironment.MongoConnectionString
            })
        {

        }

        [TestMethod]
        public void ShouldInsertOneBookWhenNoTransaction()
        {
            var entityStore = this.GetService<IEntityStore<Book>>();
            var beforeCount = entityStore.Count(p=>p.Id>0);
            var bookService = this.GetService<IBookService>();
            try
            {
                bookService.AddTwoBookWithNoTransaction();
            }
            catch
            {
                var afterCount = entityStore.Count(p => p.Id > 0);
                Assert.IsTrue(afterCount - beforeCount == 1);
            }
        }
        [TestMethod]
        public void ShouldInsertZeroBookWhenWithTransaction()
        {
            var entityStore = this.GetService<IEntityStore<Book>>();
            var beforeCount = entityStore.Count(p => p.Id > 0);
            var bookService = this.GetService<IBookService>();
            try
            {
                bookService.AddTwoBookWithTransaction();
            }
            catch
            {
                var afterCount = entityStore.Count(p => p.Id > 0);
                Assert.AreEqual(afterCount ,beforeCount);
            }
        }
        public interface IBookService
        {
            void AddTwoBookWithNoTransaction();
            void AddTwoBookWithTransaction();
        }

        [Service]
        public class BookService : IBookService
        {
            private readonly IEntityStore<Book> entityStore;

            public BookService(IEntityStore<Book> entityStore)
            {
                this.entityStore = entityStore;
            }
            public void AddBook()
            {

            }
            [Transaction]
            public void AddTwoBookWithTransaction()
            {
                entityStore.Add(new Book { Id = DateTime.Now.Ticks, Name = "book" });
                throw new ApplicationException("throw some exception");
                entityStore.Add(new Book { Id = DateTime.Now.Ticks, Name = "book" });
            }
            public void AddTwoBookWithNoTransaction()
            {
                entityStore.Add(new Book { Id = DateTime.Now.Ticks, Name = "book" });
                throw new ApplicationException("throw some exception");
                entityStore.Add(new Book { Id = DateTime.Now.Ticks, Name = "book" });
            }
        }


        #region BookContext
        public class Book
        {
            public long Id { get; set; }
            public string Name { get; set; }
        }
        [MongoContext("book_db", RegisterEntityStore = true)]
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
