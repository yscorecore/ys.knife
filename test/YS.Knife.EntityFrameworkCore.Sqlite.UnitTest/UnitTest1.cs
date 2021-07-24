﻿using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.EntityFrameworkCore.Sqlite.UnitTest.Contexts;
using YS.Knife.Hosting;
namespace YS.Knife.EntityFrameworkCore.Sqlite.UnitTest
{
    [TestClass]
    public class UnitTest1 : KnifeHost
    {

        [TestMethod]
        public void ShouldGetBloggingContext()
        {
            Assert.IsNotNull(this.GetService<BloggingContext>());
        }

        [TestMethod]
        public void ShouldGetEntityStore()
        {
            Assert.IsNotNull(this.GetService<IEntityStore<Blog>>());
            Assert.IsNotNull(this.GetService<IEntityStore<Post>>());
        }
        [TestMethod]
        public void ShouldEntityStoreDoCRUD()
        {
            var context = this.GetService<BloggingContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            var service = this.GetService<IBlogingServices>();
            service.AddTwoBlog();
            var all = service.AllBlogs();
            Assert.AreEqual(2, all.Count);
        }
        [TestMethod]
        public void ShouldListTopBlogsBySql()
        {
            var context = this.GetService<BloggingContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            var service = this.GetService<IBlogingServices>();
            service.AddTwoBlog();
            var all = service.TopBlogs(1);
            Assert.AreEqual(1, all.Count);
        }


    }
}
