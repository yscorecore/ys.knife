using Knife.Hosting.MSTest;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Knift
{
    [TestClass]
    public class ConfigurationExtensionsTest: TestBase<IConfiguration>
    {
        [TestMethod]
        public void ShouldGetNullWhenGetConnectionInfoGivenANotExistsKey()
        {
            Assert.IsNull(this.TestObject.GetConnectionInfo("ANotExistsKey"));
        }

        [TestMethod]
        public void ShouldGetValueWhenGetConnectionInfoGivenAbc()
        {
            var connectionInfo = this.TestObject.GetConnectionInfo("Abc");
            Assert.IsNotNull(connectionInfo);
            Assert.AreEqual("mssql", connectionInfo.DBType);
            Assert.AreEqual("abcValue", connectionInfo.Value);
           
        }

        [TestMethod]
        public void ShouldGetValueWhenGetConnectionInfoGivenBcd()
        {
            var connectionInfo = this.TestObject.GetConnectionInfo("Bcd");
            Assert.IsNull(connectionInfo);
        }

        [TestMethod]
        public void ShouldGetValueWhenGetConnectionInfoGivenCde()
        {
            var connectionInfo = this.TestObject.GetConnectionInfo("Cde");
            Assert.IsNotNull(connectionInfo);
            Assert.AreEqual("sqlserver", connectionInfo.DBType);
            Assert.AreEqual("Data Source=.;User ID=sa;Database=SequenceContext;Password=;", connectionInfo.Value);
        }
    }
}
