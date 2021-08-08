using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using YS.Knife.Data.Query;

namespace YS.Knife.Data.Filter
{
    
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
                new User{ Id="005",Name="",Age=21 }
            }.AsQueryable();
        }
        [Theory]
        [InlineData("Name", Operator.Equals, "LiSi", "002")]
        [InlineData("Name", Operator.Equals, "", "005")]
        [InlineData("Score", Operator.Equals, 81, "002")]
        [InlineData("Score", Operator.Equals, null, "004,005")]
        [InlineData("Age", Operator.Equals, 19, "001,004")]
        [InlineData("Age", Operator.Equals, "19", "001,004")]
        [InlineData("Age", Operator.Equals, 19.0, "001,004")]

        [InlineData("Score", Operator.NotEquals, 61, "002,003,004,005")]
        [InlineData("Score", Operator.NotEquals, null, "001,002,003")]
        [InlineData("Age", Operator.NotEquals, 19, "002,003,005")]

        [InlineData("Score", Operator.GreaterThan, 80, "002")]
        [InlineData("Age", Operator.GreaterThan, 20, "005")]
        [InlineData("Name", Operator.GreaterThan, "WangMaZi", "001,003")]
        [InlineData("Score", Operator.GreaterThanOrEqual, 80, "002")]
        [InlineData("Age", Operator.GreaterThanOrEqual, "20", "002,003,005")]
        [InlineData("Name", Operator.GreaterThanOrEqual, "WangMaZi", "001,003,004")]
        [InlineData("Score", Operator.LessThan, 70, "001")]
        [InlineData("Age", Operator.LessThan, 20, "001,004")]
        [InlineData("Score", Operator.LessThanOrEqual, 70, "001,003")]
        [InlineData("Age", Operator.LessThanOrEqual, 20, "001,002,003,004")]


        [InlineData("Name", Operator.Contains, "a", "001,003,004")]
        [InlineData("Name", Operator.NotContains, "a", "002,005")]

        [InlineData("Name", Operator.StartsWith, "W", "003,004")]
        [InlineData("Name", Operator.NotStartsWith, "W", "001,002,005")]

        [InlineData("Name", Operator.EndsWith, "i", "002,004")]
        [InlineData("Name", Operator.NotEndsWith, "i", "001,003,005")]

        //[InlineData("Age", Operator.In, new object[] { 19, "21" }, "001,004,005")]
        //[InlineData("Age", Operator.NotIn, new object[] { 19, "21" }, "002,003")]
        //[InlineData("Age", Operator.In, new object[] { }, "")]
        //[InlineData("Age", Operator.In, new object[] { 19, "21" }, "001,004,005")]
        //[InlineData("Age", Operator.NotIn, new object[] { 19, "21" }, "002,003")]

        [InlineData("Age", Operator.Between, new object[] { 19, 20 }, "001,002,003,004")]
        //[InlineData("Age", Operator.NotBetween, new object[] { 19, 20 }, "005")]
        [InlineData("Age", Operator.Between, new object[] { 20, null }, "002,003,005")]
        [InlineData("Age", Operator.Between, new object[] { null, 20 }, "001,002,003,004")]
        public void ShouldGetExpectedResultWhenFilterSingleItem(string fieldName, Operator filterType, object value, string expectedIds)
        {

            var filter = FilterInfo.CreateItem(fieldName, filterType, value);
            var ids = CreateTestUsers().DoFilter(filter).Select(p => p.Id);
            string.Join(",", ids).Should().Be(expectedIds);
        }
        [Theory]
        //[ExpectedException(typeof(FieldInfo2ExpressionException))]
        [InlineData("Age", Operator.Equals, null)]
        public void ShouldThrowFilterInfoExpressionExceptionWhenFilterSingleItemAndWithInvalidArguments(string fieldName, Operator filterType, object value)
        {
            //var filter = FilterInfo.CreateItem(fieldName, filterType, value);
            //CreateTestUsers().WhereCondition(filter).ToList();
        }


        [Fact]
        public void ShouldSupportFilterTypeExists()
        {
            //var datas = CreateUsersWithAddress();
            //var filter = FilterInfo.CreateItem("Addresses", Operator.Exists, FilterInfo.CreateItem("City", Operator.Equals, "xian"));
            //var ids = datas.WhereCondition(filter).DoSelect(p => p.Id);
            // ids).Should().Be("001,002,004", string.Join(",");
        }

        [Fact]
        public void ShouldSupportFilterTypeNotExists()
        {
            //var datas = CreateUsersWithAddress();
            //var filter = FilterInfo.CreateItem("Addresses", Operator.NotExists, FilterInfo.CreateItem("City", Operator.Equals, "xian"));
            //var ids = datas.WhereCondition(filter).DoSelect(p => p.Id);
            // ids).Should().Be("003,005", string.Join(",");
        }

        [Fact]
        public void ShouldSupportFilterTypeAll()
        {
            //var datas = CreateUsersWithAddress();
            //var filter = FilterInfo.CreateItem("Addresses", Operator.All, FilterInfo.CreateItem("City", Operator.Equals, "xian"));
            //var ids = datas.WhereCondition(filter).DoSelect(p => p.Id);
            // ids).Should().Be("001,004", string.Join(",");
        }
        [Fact]
        public void ShouldSupportFilterTypeNotAll()
        {
            //var datas = CreateUsersWithAddress();
            //var filter = FilterInfo.CreateItem("Addresses", Operator.NotAll, FilterInfo.CreateItem("City", Operator.Equals, "xian"));
            //var ids = datas.WhereCondition(filter).DoSelect(p => p.Id);
            // ids).Should().Be("002,003,005", string.Join(",");
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
