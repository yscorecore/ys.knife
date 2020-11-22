using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.EntityFrameworkCore.Sqlite.UnitTest.Contexts
{
    [Service(Lifetime = ServiceLifetime.Scoped)]
    public class BlogingServices : IBlogingServices
    {
        IEntityStore<Blog> blogStore;
        public BlogingServices(IEntityStore<Blog> store)
        {
            this.blogStore = store;
        }
        [CommitEFChanges]
        public void AddTwoBlog()
        {
            blogStore.Add(new Blog { Url = "http://www.baidu.com" });
            blogStore.Add(new Blog { Url = "http://www.sina.com" });
        }

        public List<Blog> AllBlogs()
        {
            return blogStore.Query(null).ToList();
        }
    }

}
