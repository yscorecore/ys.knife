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
            OrderInfo order = new OrderItem("Name");
            order.Add("Age", OrderType.Desc);
            Assert.AreEqual("+Name,-Age", order.ToString());
        }

        [TestMethod]
        public void ShouldGetExpectedOrderInfoWhenParseFromString()
        {
            var orderInfo = OrderInfo.Parse("+Name, -Age ,, Address ");
            Assert.IsTrue(orderInfo.HasItems());
            Assert.AreEqual(3, orderInfo.Items.Count);
            AssertOrderItem(orderInfo.Items[0], "Name", OrderType.Asc);
            AssertOrderItem(orderInfo.Items[1], "Age", OrderType.Desc);
            AssertOrderItem(orderInfo.Items[2], "Address", OrderType.Asc);
        }

        private void AssertOrderItem(OrderItem orderItem, string field, OrderType orderType)
        {
            Assert.AreEqual(field, orderItem.FieldName);
            Assert.AreEqual(orderType, orderItem.OrderType);
        }

        [TestMethod]
        public void ShouldCanConvertToString()
        {
            var converter = TypeDescriptor.GetConverter(typeof(OrderInfo));
            Assert.AreEqual(true, converter.CanConvertTo(typeof(string)));
            OrderInfo orderItem = new OrderInfo().Add("Field", OrderType.Desc);
            Assert.AreEqual("-Field", converter.ConvertTo(orderItem, typeof(string)));
        }
        [TestMethod]
        public void ShouldCanConvertFromString()
        {
            var converter = TypeDescriptor.GetConverter(typeof(OrderInfo));
            Assert.AreEqual(true, converter.CanConvertFrom(typeof(string)));
            Assert.IsInstanceOfType(converter.ConvertFrom("-Field,-Field2"), typeof(OrderInfo));
        }
    }
}
