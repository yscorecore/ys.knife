using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Knife.Data.UnitTest
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
        [DataRow("Name", FilterType.Equals, "LiSi", "002")]
        [DataRow("Name", FilterType.Equals, null, "005")]
        [DataRow("Score", FilterType.Equals, 81, "002")]
        [DataRow("Score", FilterType.Equals, null, "004,005")]
        [DataRow("Age", FilterType.Equals, 19, "001,004")]
        [DataRow("Age", FilterType.Equals, "19", "001,004")]
        [DataRow("Age", FilterType.Equals, 19.0, "001,004")]

        [DataRow("Score", FilterType.NotEquals, 61, "002,003,004,005")]
        [DataRow("Score", FilterType.NotEquals, null, "001,002,003")]
        [DataRow("Age", FilterType.NotEquals, 19, "002,003,005")]

        [DataRow("Score", FilterType.GreaterThan, 80, "002")]
        [DataRow("Age", FilterType.GreaterThan, 20, "005")]
        [DataRow("Name", FilterType.GreaterThan, "WangMaZi", "001,003")]
        [DataRow("Score", FilterType.GreaterThanOrEqual, 80, "002")]
        [DataRow("Age", FilterType.GreaterThanOrEqual, 20, "002,003,005")]
        [DataRow("Name", FilterType.GreaterThanOrEqual, "WangMaZi", "001,003,004")]
        [DataRow("Score", FilterType.LessThan, 70, "001")]
        [DataRow("Age", FilterType.LessThan, 20, "001,004")]
        [DataRow("Score", FilterType.LessThanOrEqual, 70, "001,003")]
        [DataRow("Age", FilterType.LessThanOrEqual, 20, "001,002,003,004")]
        public void ShouldGetExpectedResultWhenFilterSingleItem(string fieldName, FilterType filterType, object value, string expectedIds)
        {
            var filter = FilterInfo.CreateItem(fieldName, filterType, value);
            var ids = CreateTestUsers().WhereCondition(filter)
                    .Select(p => p.Id);
            Assert.AreEqual(expectedIds, string.Join(",", ids));
        }
        [DataTestMethod]
        [ExpectedException(typeof(FilterInfoExpressionException))]
        [DataRow("Age", FilterType.Equals, null)]
        public void ShouldThrowFilterInfoExpressionExceptionWhenFilterSingleItemAndWithInvalidArguments(string fieldName, FilterType filterType, object value)
        {
            var filter = FilterInfo.CreateItem(fieldName, filterType, value);
            CreateTestUsers().WhereCondition(filter).ToList();
        }

       
        public class User
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }

            public int? Score { get; set; }
        }
    }
}
