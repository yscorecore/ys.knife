using System;
using System.Threading;
using System.Threading.Tasks;
using Knife.Hosting;
using Knife.Hosting.MSTest;
using Knife.Test;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.EntityFrameworkCore.MySql.UnitTest.TestData;

namespace YS.Knife.EntityFrameworkCore.MySql.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ShouldNotBeNullWhenHasDefineMySqlDbContextClassAndNoInjectType()
        {
            using (Utility.WithNetcoreEnv(""))
            {
                using (var host = new KnifeHost())
                {
                    Assert.IsNotNull(host.Get<TestDbContext>());
                }
            }
        }

        [TestMethod]
        public void ShouldNotBeNullWhenHasDefineMySqlDbContextClassAndWithInjectType()
        {
            using (Utility.WithNetcoreEnv(""))
            {
                using (var host = new KnifeHost())
                {
                    Assert.IsNotNull(host.Get<TestDbContext2>());
                }
            }
        }
    }


}
