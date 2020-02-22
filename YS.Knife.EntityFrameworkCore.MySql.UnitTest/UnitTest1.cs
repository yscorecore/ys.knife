using Knife.Hosting.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.EntityFrameworkCore.MySql.UnitTest.TestData;

namespace YS.Knife.EntityFrameworkCore.MySql.UnitTest
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
