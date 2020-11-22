using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace YS.Knife.EntityFrameworkCore.Sqlite.UnitTest.Contexts
{
    public class BloggingContextDesignTimeFactory : SqliteDesignTimeDbContextFactory<BloggingContext>
    {
    }
}
