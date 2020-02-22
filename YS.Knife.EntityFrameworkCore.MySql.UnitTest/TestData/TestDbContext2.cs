using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.EntityFrameworkCore.MySql.UnitTest.TestData
{
    [MySqlDbContextClass("TestContext2", InjectType = typeof(TestDbContext2))]
    public class TestDbContext2 : DbContext
    {
        public TestDbContext2(DbContextOptions<TestDbContext2> dbContextOptions) : base(dbContextOptions)
        {

        }
    }
}
