using YS.Knife.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Knift
{
    [TestClass]
    public class ConfigurationExtensionsTest: KnifeHost
    {
        static Dictionary<string, object> CommandArguments = new Dictionary<string, object>
        {
            ["ConnectionStrings:@DbType"] = "mssql",
            ["ConnectionStrings:Abc"] = "abcValue",
            ["ConnectionStrings:Bcd"] = "",
            ["ConnectionStrings:Cde"] = "sqlserver#Data Source=.;User ID=sa;Database=SequenceContext;Password=;",
        };
        public ConfigurationExtensionsTest():base(CommandArguments)
        {
            this.configuration = GetService<IConfiguration>();
        }
        private IConfiguration configuration;
        [TestMethod]
        public void ShouldGetNullWhenGetConnectionInfoGivenANotExistsKey()
        {
            Assert.IsNull(this.configuration.GetConnectionInfo("ANotExistsKey"));
        }

        [TestMethod]
        public void ShouldGetValueWhenGetConnectionInfoGivenAbc()
        {
            var connectionInfo = this.configuration.GetConnectionInfo("Abc");
            Assert.IsNotNull(connectionInfo);
            Assert.AreEqual("mssql", connectionInfo.DBType);
            Assert.AreEqual("abcValue", connectionInfo.Value);
           
        }

        [TestMethod]
        public void ShouldGetValueWhenGetConnectionInfoGivenBcd()
        {
            var connectionInfo = this.configuration.GetConnectionInfo("Bcd");
            Assert.IsNull(connectionInfo);
        }

        [TestMethod]
        public void ShouldGetValueWhenGetConnectionInfoGivenCde()
        {
            var connectionInfo = this.configuration.GetConnectionInfo("Cde");
            Assert.IsNotNull(connectionInfo);
            Assert.AreEqual("sqlserver", connectionInfo.DBType);
            Assert.AreEqual("Data Source=.;User ID=sa;Database=SequenceContext;Password=;", connectionInfo.Value);
        }
    }
}
