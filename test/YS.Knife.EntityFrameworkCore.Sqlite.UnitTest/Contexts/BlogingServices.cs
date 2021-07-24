using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.EntityFrameworkCore.Sqlite.UnitTest.Contexts
{
    [Service(Lifetime = ServiceLifetime.Scoped)]
    public class BlogingServices : IBlogingServices
    {
        readonly IEntityStore<Blog> blogStore;
        readonly IBlogSql blogSql;


        public BlogingServices(IEntityStore<Blog> store, IBlogSql blogSql)
        {
            this.blogStore = store;
            this.blogSql = blogSql;
        }
        [Transaction]
        public void AddTwoBlog()
        {
            blogStore.Add(new Blog { Url = "http://www.baidu.com" });
            blogStore.Add(new Blog { Url = "http://www.sina.com" });
        }

        public List<Blog> AllBlogs()
        {

            return blogStore.Query(null).ToList();
        }

        public List<Blog> TopBlogs(int limit)
        {
            return blogSql.QueryBySql(limit).ToList();
        }
    }

}
