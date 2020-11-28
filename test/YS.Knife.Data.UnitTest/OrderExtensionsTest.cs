using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace YS.Knife.Data.UnitTest
{
    [TestClass]
    public class OrderExtensionsTest
    {
        [TestMethod]
        public void ShouldOrderByWhenGiveIdAsc()
        {
            var data = CreateTestUsers().Order(new OrderItem("Id", OrderType.Asc))
                 .Select(p => p.Id).ToList();
            Assert.AreEqual("001,002,003,004", string.Join(",", data));
        }
        [TestMethod]
        public void ShouldOrderByWhenGiveIdDesc()
        {
            var data = CreateTestUsers().Order(new OrderItem("Id", OrderType.Desc))
                 .Select(p => p.Id).ToList();
            Assert.AreEqual("004,003,002,001", string.Join(",", data));
        }

        [TestMethod]
        public void ShouldOrderByGiveAgeDescAndNameAsc()
        {
            var orderInfo = new OrderInfo()
                .Add("Age", OrderType.Desc)
                .Add("Name", OrderType.Asc);
            var data = CreateTestUsers().Order(orderInfo)
                 .Select(p => p.Id).ToList();
            Assert.AreEqual("002,003,004,001", string.Join(",", data));
        }

        [TestMethod]
        public void ShouldOrderByGiveAgeAscAndNameDesc()
        {
            var orderInfo = new OrderInfo()
                .Add("Age", OrderType.Asc)
                .Add("Name", OrderType.Desc);
            var data = CreateTestUsers().Order(orderInfo)
                 .Select(p => p.Id).ToList();
            Assert.AreEqual("001,004,003,002", string.Join(",", data));
        }

        private IQueryable<User> CreateTestUsers()
        {
            return new List<User>
            {
                new User{ Id="001",Name="ZhangSan",Age=19 },
                new User{ Id="002",Name="LiSi",Age=20 },
                new User{ Id="003",Name="WangWu",Age=20 },
                new User{ Id="004",Name="WangMaZi",Age=19 }
            }.AsQueryable();
        }

        public class User
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
        }
    }
}
