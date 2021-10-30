using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using YS.Knife.EntityFrameworkCore.Sqlite.UnitTest.Contexts;
using YS.Knife.Hosting;
namespace YS.Knife.EntityFrameworkCore.Sqlite.UnitTest
{

    public class UnitTest1 : KnifeHost
    {

        [Fact]
        public void ShouldGetBloggingContext()
        {
            this.GetService<BloggingContext>().Should().NotBeNull();
        }

        [Fact]
        public void ShouldGetEntityStore()
        {
            this.GetService<IEntityStore<Blog>>().Should().NotBeNull();
            this.GetService<IEntityStore<Post>>().Should().NotBeNull();
        }
        [Fact]
        public void ShouldEntityStoreDoCRUD()
        {
            var context = this.GetService<BloggingContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            var service = this.GetService<IBlogingServices>();
            service.AddTwoBlog();
            var all = service.AllBlogs();
            all.Count.Should().Be(2);
        }
        [Fact]
        public void ShouldListTopBlogsBySql()
        {
            var context = this.GetService<BloggingContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            var service = this.GetService<IBlogingServices>();
            service.AddTwoBlog();
            var all = service.TopBlogs(1);
            all.Count.Should().Be(1);
        }


    }
}
