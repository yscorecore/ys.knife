using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Knife.Data.UnitTest
{
    [TestClass]
    public class OrderItemTest
    {
        [DataTestMethod]
        [DataRow("Field1", OrderType.Asc, "Field1__asc")]
        [DataRow("Field1", OrderType.Desc, "Field1__desc")]
        public void ShouldGetExpectedStringWhenToString(string field, OrderType orderType, string expected)
        {
            var orderItem = new OrderItem { FieldName = field, OrderType = orderType };
            Assert.AreEqual(expected, orderItem.ToString());
        }
        [DataTestMethod]
        [DataRow("Field1__asc", "Field1", OrderType.Asc)]
        [DataRow("Field1__ASC", "Field1", OrderType.Asc)]
        [DataRow("Field1", "Field1", OrderType.Asc)]
        [DataRow("Field1__desc", "Field1", OrderType.Desc)]
        [DataRow("Field1__DESC", "Field1", OrderType.Desc)]
        [DataRow("Field1__other", "Field1__other", OrderType.Asc)]
        public void ShouldGetExpectedFieldAndOrderTypeWhenParseOrderItemFromString(string text, string expectedField, OrderType expectedOrderType)
        {
            var orderItem = OrderItem.Parse(text);
            Assert.AreEqual(expectedField, orderItem.FieldName);
            Assert.AreEqual(expectedOrderType, orderItem.OrderType);
        }

    }
}
