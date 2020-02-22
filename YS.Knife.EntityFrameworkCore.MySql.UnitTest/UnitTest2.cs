using Knife.Hosting.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.EntityFrameworkCore.MySql.UnitTest.TestData;

namespace YS.Knife.EntityFrameworkCore.MySql.UnitTest
{
    [TestClass]
    public class UnitTest2:TestBase<TestDbContext2>
    {
        [TestMethod]
        public void ShouldNotBeNullWhenHasDefineMySqlDbContextClassAndWithInjectType()
        {
            Assert.IsNotNull(this.TestObject);
        }
    }
}
