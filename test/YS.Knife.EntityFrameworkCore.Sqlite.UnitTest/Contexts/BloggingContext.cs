using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace YS.Knife.EntityFrameworkCore.Sqlite.UnitTest.Contexts
{
    [SqliteEFContext("blogging")]
    public class BloggingContext : DbContext
    {
        public BloggingContext(DbContextOptions<BloggingContext> options) : base(options)
        {

        }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
        // public DbSet<PostCount> PostCounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
    [Keyless]
    public class PostCount
    {
        public int BlogName { get; set; }
        public int TotalCount { get; set; }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }

        public List<Post> Posts { get; } = new List<Post>();
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}
