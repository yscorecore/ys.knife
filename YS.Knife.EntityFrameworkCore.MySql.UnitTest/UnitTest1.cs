using System;
using System.Threading;
using System.Threading.Tasks;
using Knife.Hosting;
using Knife.Test;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.EntityFrameworkCore.MySql.UnitTest.TestData;
using System.Collections.Generic;

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
                Assert.IsNotNull(host.Get<TestDbContext>());
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
                Assert.IsNotNull(host.Get<TestDbContext2>());
            }

        }
    }


}
