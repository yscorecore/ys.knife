using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Knife.Data.UnitTest
{
    [TestClass]
    public class OrderItemTest
    {
        [DataTestMethod]
        [DataRow("Field1", OrderType.Asc, "+Field1")]
        [DataRow("Field1", OrderType.Desc, "-Field1")]
        public void ShouldGetExpectedStringWhenToString(string field, OrderType orderType, string expected)
        {
            var orderItem = new OrderItem(field, orderType);
            Assert.AreEqual(expected, orderItem.ToString());
        }
        [DataTestMethod]
        [DataRow("+Field1", "Field1",OrderType.Asc)]
        [DataRow("Field1", "Field1", OrderType.Asc)]
        [DataRow("-Field1", "Field1", OrderType.Desc)]
        [DataRow("++Field1", "Field1", OrderType.Asc)]
        [DataRow("+-Field1", "Field1", OrderType.Desc)]
        [DataRow("--Field1", "Field1", OrderType.Desc)]
        public void ShouldGetExpectedFieldAndOrderTypeWhenParseOrderItemFromString(string text, string expectedField, OrderType expectedOrderType)
        {
            var orderItem = OrderItem.Parse(text);
            Assert.AreEqual(expectedField, orderItem.FieldName);
            Assert.AreEqual(expectedOrderType, orderItem.OrderType);
        }
        [TestMethod]
        public void ShouldCanConvertToString()
        {
            var converter = TypeDescriptor.GetConverter(typeof(OrderItem));
            Assert.AreEqual(true, converter.CanConvertTo(typeof(string)));
            var orderItem = new OrderItem("Field", OrderType.Desc);
            Assert.AreEqual("-Field", converter.ConvertTo(orderItem, typeof(string)));
        }
        [TestMethod]
        public void ShouldCanConvertFromString()
        {
            var converter = TypeDescriptor.GetConverter(typeof(OrderItem));
            Assert.AreEqual(true, converter.CanConvertFrom(typeof(string)));
            Assert.IsInstanceOfType(converter.ConvertFrom("-Field"), typeof(OrderItem));
        }
    }
}
