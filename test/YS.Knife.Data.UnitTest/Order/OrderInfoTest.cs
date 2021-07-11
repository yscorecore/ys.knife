using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Knife.Data.UnitTest
{
    [TestClass]
    public class OrderInfoTest
    {


        [TestMethod]
        public void ShouldGetExpectedStringWhenToString()
        {
            OrderInfo order = OrderInfo.Create("Name")
                    .Add("Age", OrderType.Desc);
            Assert.AreEqual("Name__asc,Age__desc", order.ToString());
        }

        [TestMethod]
        public void ShouldGetExpectedOrderInfoWhenParseFromString()
        {
            var orderInfo = OrderInfo.Parse("Name, Age__desc ,Address__asc ");
            Assert.IsTrue(orderInfo.HasItems());
            Assert.AreEqual(3, orderInfo.Items.Count);
            AssertOrderItem(orderInfo.Items[0], "Name", OrderType.Asc);
            AssertOrderItem(orderInfo.Items[1], "Age", OrderType.Desc);
            AssertOrderItem(orderInfo.Items[2], "Address", OrderType.Asc);
        }

        private void AssertOrderItem(OrderItem2 orderItem, string field, OrderType orderType)
        {
            Assert.AreEqual(field, orderItem.ToString());
            Assert.AreEqual(orderType, orderItem.OrderType);
        }

        [TestMethod]
        public void ShouldCanConvertToString()
        {
            var converter = TypeDescriptor.GetConverter(typeof(OrderInfo));
            Assert.AreEqual(true, converter.CanConvertTo(typeof(string)));
            OrderInfo orderItem = new OrderInfo().Add("Field", OrderType.Desc);
            Assert.AreEqual("Field__desc", converter.ConvertTo(orderItem, typeof(string)));
        }
        [TestMethod]
        public void ShouldCanConvertFromString()
        {
            var converter = TypeDescriptor.GetConverter(typeof(OrderInfo));
            Assert.AreEqual(true, converter.CanConvertFrom(typeof(string)));
            Assert.IsInstanceOfType(converter.ConvertFrom("Field__desc,Field2__desc"), typeof(OrderInfo));
        }
    }
}
