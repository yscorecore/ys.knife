using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Knife.Hosting
{
    [TestClass]
    public class InjectConfigurationTest : KnifeHost
    {
       

        [InjectConfiguration("connectionstrings:conn1")]
        private readonly string ConnectionString1 = "conn1";

        [InjectConfiguration("connectionstrings:conn2")]
        public string ConnectionString2 = "conn2";

        [InjectConfiguration("connectionstrings:conn3")]
        private string ConnectionString3 { get; set; }= "conn3";

        [InjectConfiguration("connectionstrings:conn4")]
        public string ConnectionString4 { get; set; } = "conn4";

        [InjectConfiguration("connectionstrings")]
        private IDictionary<string, object> Connections = new Dictionary<string, object>
        {
            ["conn5"] = "conn5",
            ["conn6"] = 123456
        };

        [DataRow("conn1", "conn1")]
        [DataRow("conn2", "conn2")]
        [DataRow("conn3", "conn3")]
        [DataRow("conn4", "conn4")]
        [DataRow("conn5", "conn5")]
        [DataRow("conn6", "123456")]
        [DataTestMethod]

        public void ShouldGetConnectionStringsFromInjectValue(string key, string expectedValue)
        {
            var configuration = this.GetService<IConfiguration>();
            Assert.AreEqual(expectedValue, configuration.GetConnectionString(key));
        }

        [TestMethod]
        public void ShouldHaveHighestPriority()
        { 
        
        }
    }
}
