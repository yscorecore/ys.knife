using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YS.Knife.EntityFrameworkCore.Sqlite.UnitTest.Contexts;

namespace YS.Knife.EntityFrameworkCore.Sqlite.UnitTest
{
    [RowSql(typeof(BloggingContext))]
    public interface IBlogSql
    {
        [SqlQuery("select * from blogs limit {0}")]
        IQueryable<Blog> QueryBySql(int limit);
    }
}
