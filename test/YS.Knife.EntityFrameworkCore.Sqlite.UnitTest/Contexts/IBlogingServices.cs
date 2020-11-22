using System.Collections.Generic;

namespace YS.Knife.EntityFrameworkCore.Sqlite.UnitTest.Contexts
{
    public interface IBlogingServices
    {
        void AddTwoBlog();
        List<Blog> AllBlogs();
    }
}