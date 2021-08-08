﻿using System;
using FluentAssertions;
using MongoDB.Driver;
using Xunit;
using YS.Knife.Entity;
using YS.Knife.Hosting;
namespace YS.Knife.Mongo.UnitTest
{

    [Collection(nameof(TestEnvironment))]
    public class TransactionAttributeTest : KnifeHost
    {


        [InjectConfiguration("connectionStrings:book_db")]
        private readonly string _ = TestEnvironment.MongoConnectionString;

        [Fact]
        public void ShouldInsertOneBookWhenNoTransactionAndThrowError()
        {
            var entityStore = this.GetService<IEntityStore<Book>>();
            var beforeCount = entityStore.Count(p => p.Id > 0);
            var bookService = this.GetService<IBookService>();
            try
            {
                bookService.AddTwoBookNoTransaction(true);
            }
            catch
            {
                var afterCount = entityStore.Count(p => p.Id > 0);
                (afterCount - beforeCount).Should().Be(1);
            }
        }
        [Fact]
        public void ShouldInsertTwoBookWhenNoTransaction()
        {
            var entityStore = this.GetService<IEntityStore<Book>>();
            var beforeCount = entityStore.Count(p => p.Id > 0);
            var bookService = this.GetService<IBookService>();
            bookService.AddTwoBookNoTransaction(false);
            var afterCount = entityStore.Count(p => p.Id > 0);
            (afterCount - beforeCount).Should().Be(2);
        }

        [Fact]
        public void ShouldInsertZeroBookWhenWithTransactionAndThrowError()
        {
            var entityStore = this.GetService<IEntityStore<Book>>();
            var beforeCount = entityStore.Count(p => p.Id > 0);
            var bookService = this.GetService<IBookService>();
            try
            {
                bookService.AddTwoBookWithTransaction(true);
            }
            catch
            {
                var afterCount = entityStore.Count(p => p.Id > 0);
                beforeCount.Should().Be(afterCount);
            }
        }
        [Fact]
        public void ShouldInsertTwoBookWhenWithTransaction()
        {
            var entityStore = this.GetService<IEntityStore<Book>>();
            var beforeCount = entityStore.Count(p => p.Id > 0);
            var bookService = this.GetService<IBookService>();
            bookService.AddTwoBookNoTransaction(false);
            var afterCount = entityStore.Count(p => p.Id > 0);
            (afterCount - beforeCount).Should().Be(2);
        }

        public interface IBookService
        {
            void AddTwoBookNoTransaction(bool throwError);
            void AddTwoBookWithTransaction(bool throwError);
        }

        [Service]
        public class BookService : IBookService
        {
            private readonly IEntityStore<Book> entityStore;

            public BookService(IEntityStore<Book> entityStore)
            {
                this.entityStore = entityStore;
            }

            [Transaction]
            public void AddTwoBookWithTransaction(bool throwError)
            {
                entityStore.Add(new Book { Id = DateTime.Now.Ticks, Name = "book" });
                if (throwError)
                    throw new ApplicationException("throw some exception");
                entityStore.Add(new Book { Id = DateTime.Now.Ticks, Name = "book" });
            }
            public void AddTwoBookNoTransaction(bool throwError)
            {
                entityStore.Add(new Book { Id = DateTime.Now.Ticks, Name = "book" });
                if (throwError)
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
