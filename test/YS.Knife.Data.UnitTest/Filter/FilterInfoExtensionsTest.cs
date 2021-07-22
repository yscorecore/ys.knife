using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Knife.Data.Filter
{
    [TestClass]
    public class FilterInfoExtensionsTest
    {

        private static IQueryable<User> CreateTestUsers()
        {
            return new List<User>
            {
                new User{ Id="001",Name="ZhangSan",Age=19,Score=61 },
                new User{ Id="002",Name="LiSi",Age=20,Score=81 },
                new User{ Id="003",Name="WangWu",Age=20,Score=70 },
                new User{ Id="004",Name="WangMaZi",Age=19 },
                new User{ Id="005",Name=null,Age=21 }
            }.AsQueryable();
        }
        [DataTestMethod]
        [DataRow("Name", Operator.Equals, "LiSi", "002")]
        [DataRow("Name", Operator.Equals, null, "005")]
        [DataRow("Score", Operator.Equals, 81, "002")]
        [DataRow("Score", Operator.Equals, null, "004,005")]
        [DataRow("Age", Operator.Equals, 19, "001,004")]
        [DataRow("Age", Operator.Equals, "19", "001,004")]
        [DataRow("Age", Operator.Equals, 19.0, "001,004")]

        [DataRow("Score", Operator.NotEquals, 61, "002,003,004,005")]
        [DataRow("Score", Operator.NotEquals, null, "001,002,003")]
        [DataRow("Age", Operator.NotEquals, 19, "002,003,005")]

        [DataRow("Score", Operator.GreaterThan, 80, "002")]
        [DataRow("Age", Operator.GreaterThan, 20, "005")]
        [DataRow("Name", Operator.GreaterThan, "WangMaZi", "001,003")]
        [DataRow("Score", Operator.GreaterThanOrEqual, 80, "002")]
        [DataRow("Age", Operator.GreaterThanOrEqual, 20, "002,003,005")]
        [DataRow("Name", Operator.GreaterThanOrEqual, "WangMaZi", "001,003,004")]
        [DataRow("Score", Operator.LessThan, 70, "001")]
        [DataRow("Age", Operator.LessThan, 20, "001,004")]
        [DataRow("Score", Operator.LessThanOrEqual, 70, "001,003")]
        [DataRow("Age", Operator.LessThanOrEqual, 20, "001,002,003,004")]


        [DataRow("Name", Operator.Contains, "a", "001,003,004")]
        [DataRow("Name", Operator.NotContains, "a", "002,005")]

        [DataRow("Name", Operator.StartsWith, "W", "003,004")]
        [DataRow("Name", Operator.NotStartsWith, "W", "001,002,005")]

        [DataRow("Name", Operator.EndsWith, "i", "002,004")]
        [DataRow("Name", Operator.NotEndsWith, "i", "001,003,005")]

        [DataRow("Age", Operator.In, new object[] { 19, "21" }, "001,004,005")]
        [DataRow("Age", Operator.NotIn, new object[] { 19, "21" }, "002,003")]
        [DataRow("Age", Operator.In, new object[] { }, "")]
        [DataRow("Age", Operator.In, new object[] { 19, "21" }, "001,004,005")]
        [DataRow("Age", Operator.NotIn, new object[] { 19, "21" }, "002,003")]

        [DataRow("Age", Operator.Between, new object[] { 19, 20 }, "001,002,003,004")]
        [DataRow("Age", Operator.NotBetween, new object[] { 19, 20 }, "005")]
        public void ShouldGetExpectedResultWhenFilterSingleItem(string fieldName, Operator filterType, object value, string expectedIds)
        {
            //var filter = FilterInfo.CreateItem(fieldName, filterType, value);
            //var ids = CreateTestUsers().WhereCondition(filter)
            //        .Select(p => p.Id);
            //Assert.AreEqual(expectedIds, string.Join(",", ids));
        }
        [DataTestMethod]
        [ExpectedException(typeof(FieldInfo2ExpressionException))]
        [DataRow("Age", Operator.Equals, null)]
        public void ShouldThrowFilterInfoExpressionExceptionWhenFilterSingleItemAndWithInvalidArguments(string fieldName, Operator filterType, object value)
        {
            //var filter = FilterInfo.CreateItem(fieldName, filterType, value);
            //CreateTestUsers().WhereCondition(filter).ToList();
        }


        [TestMethod]
        public void ShouldSupportFilterTypeExists()
        {
            //var datas = CreateUsersWithAddress();
            //var filter = FilterInfo.CreateItem("Addresses", Operator.Exists, FilterInfo.CreateItem("City", Operator.Equals, "xian"));
            //var ids = datas.WhereCondition(filter).Select(p => p.Id);
            //Assert.AreEqual("001,002,004", string.Join(",", ids));
        }

        [TestMethod]
        public void ShouldSupportFilterTypeNotExists()
        {
            //var datas = CreateUsersWithAddress();
            //var filter = FilterInfo.CreateItem("Addresses", Operator.NotExists, FilterInfo.CreateItem("City", Operator.Equals, "xian"));
            //var ids = datas.WhereCondition(filter).Select(p => p.Id);
            //Assert.AreEqual("003,005", string.Join(",", ids));
        }

        [TestMethod]
        public void ShouldSupportFilterTypeAll()
        {
            //var datas = CreateUsersWithAddress();
            //var filter = FilterInfo.CreateItem("Addresses", Operator.All, FilterInfo.CreateItem("City", Operator.Equals, "xian"));
            //var ids = datas.WhereCondition(filter).Select(p => p.Id);
            //Assert.AreEqual("001,004", string.Join(",", ids));
        }
        [TestMethod]
        public void ShouldSupportFilterTypeNotAll()
        {
            //var datas = CreateUsersWithAddress();
            //var filter = FilterInfo.CreateItem("Addresses", Operator.NotAll, FilterInfo.CreateItem("City", Operator.Equals, "xian"));
            //var ids = datas.WhereCondition(filter).Select(p => p.Id);
            //Assert.AreEqual("002,003,005", string.Join(",", ids));
        }
        public IQueryable<User> CreateUsersWithAddress()
        {
            return (new List<User>()
            {
                new User{ Id="001",Name="ZhangSan",Age=19,Score=61, Addresses = new List<Address>() {new Address(){City = "xian"}}},
                new User{ Id="002",Name="LiSi",Age=20,Score=81,Addresses = new List<Address>() {new Address(){City = "beijing"},new Address(){City = "xian"}} },
                new User{ Id="003",Name="WangWu",Age=20,Score=70,Addresses = new List<Address>() {new Address(){City = "beijing"}} },
                new User{ Id="004",Name="WangMaZi",Age=19,Addresses = new List<Address>() {new Address(){City = "xian"},new Address(){City = "xian"}} },
                new User{ Id="005",Name=null,Age=21, Addresses = new List<Address>() {new Address(){City = "beijing"},new Address(){City = "beijing"}} }
            }).AsQueryable();
        }



        public class User
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }

            public int? Score { get; set; }

            public List<Address> Addresses { get; set; } = new List<Address>();
        }
        public class Address
        {
            public string City { get; set; }
        }
    }
}
