using Knife.Hosting.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.EntityFrameworkCore.MySql.UnitTest.TestData;

namespace YS.EntityFrameworkCore.MySql.UnitTest
{
    [TestClass]
    public class UnitTest1:TestBase<TestDbContext>
    {
        [TestMethod]
        public void ShouldNotBeNullWhenHasDefineMySqlDbContextClass()
        {
            Assert.IsNotNull(this.TestObject);
        }
    }
}
