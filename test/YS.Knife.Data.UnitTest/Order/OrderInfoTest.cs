using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.Data.Query;

namespace YS.Knife.Data.UnitTest
{
    [TestClass]
    public class OrderInfoTest
    {


        [TestMethod]
        public void ShouldGetExpectedStringWhenToString()
        {
            OrderInfo order = OrderInfo.Parse("Name,Age.desc()");
            Assert.AreEqual("Name.asc(),Age.desc()", order.ToString());
        }

        [TestMethod]
        public void ShouldGetExpectedOrderInfoWhenParseFromString()
        {
            var orderInfo = OrderInfo.Parse("Name, Age.desc() ,Address.asc() ");
            Assert.IsTrue(orderInfo.HasItems());
            Assert.AreEqual(3, orderInfo.Items.Count);
            AssertOrderItem(orderInfo.Items[0], "Name", OrderType.Asc);
            AssertOrderItem(orderInfo.Items[1], "Age", OrderType.Desc);
            AssertOrderItem(orderInfo.Items[2], "Address", OrderType.Asc);
        }

        private void AssertOrderItem(OrderItem orderItem, string field, OrderType orderType)
        {
            Assert.AreEqual(field, string.Join(".", orderItem.NavigatePaths));
            Assert.AreEqual(orderType, orderItem.OrderType);
        }

        [TestMethod]
        public void ShouldCanConvertToString()
        {
            var converter = TypeDescriptor.GetConverter(typeof(OrderInfo));
            Assert.AreEqual(true, converter.CanConvertTo(typeof(string)));
            OrderInfo orderItem = OrderInfo.Parse("Field.Desc()");
            Assert.AreEqual("Field.desc()", converter.ConvertTo(orderItem, typeof(string)));
        }
        [TestMethod]
        public void ShouldCanConvertFromString()
        {
            var converter = TypeDescriptor.GetConverter(typeof(OrderInfo));
            Assert.AreEqual(true, converter.CanConvertFrom(typeof(string)));
            Assert.IsInstanceOfType(converter.ConvertFrom("Field.desc(),Field2.desc()"), typeof(OrderInfo));
        }
    }
}
