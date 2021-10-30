using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace YS.Knife.EntityFrameworkCore.Sqlite.UnitTest.Contexts
{
    [KnifeEFContext()]
    public class BloggingContext : DbContext
    {
        private readonly IConfiguration configuration;

        public BloggingContext(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public BloggingContext(DbContextOptions<BloggingContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite(configuration.GetConnectionString("blogging"));
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
