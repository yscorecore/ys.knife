using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using YS.Knife.EntityFrameworkCore.MySql.UnitTest.TestData;
using YS.Knife.Hosting;

namespace YS.Knife.EntityFrameworkCore.MySql.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ShouldNotBeNullWhenHasDefineMySqlDbContextClassAndNoInjectType()
        {
            var args = new Dictionary<string, object>
            {
                ["ConnectionStrings:TestContext"] = "Server=localhost;Database=SequenceContext; User=root;Password=;"
            };
            using (var host = new KnifeHost(args))
            {
                Assert.IsNotNull(host.GetService<TestDbContext>());
            }
        }

        [TestMethod]
        public void ShouldNotBeNullWhenHasDefineMySqlDbContextClassAndWithInjectType()
        {
            var args = new Dictionary<string, object>
            {
                ["ConnectionStrings:TestContext2"] = "Server=localhost;Database=SequenceContext; User=root;Password=;"
            };
            using (var host = new KnifeHost(args))
            {
                Assert.IsNotNull(host.GetService<TestDbContext2>());
            }

        }
    }


}
