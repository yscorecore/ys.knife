using System.ComponentModel;
using FluentAssertions;
using Xunit;
using YS.Knife.Data.Query;

namespace YS.Knife.Data.UnitTest
{

    public class OrderInfoTest
    {


        [Fact]
        public void ShouldGetExpectedStringWhenToString()
        {
            OrderInfo order = OrderInfo.Parse("Name,Age.desc()");
            order.ToString().Should().Be("Name.asc(),Age.desc()");
        }

        [Fact]
        public void ShouldGetExpectedOrderInfoWhenParseFromString()
        {
            var orderInfo = OrderInfo.Parse("Name, Age.desc() ,Address.asc() ");
            orderInfo.HasItems().Should().BeTrue();
            orderInfo.Items.Count.Should().Be(3);
            AssertOrderItem(orderInfo.Items[0], "Name", OrderType.Asc);
            AssertOrderItem(orderInfo.Items[1], "Age", OrderType.Desc);
            AssertOrderItem(orderInfo.Items[2], "Address", OrderType.Asc);
        }

        private void AssertOrderItem(OrderItem orderItem, string field, OrderType orderType)
        {
            string.Join(".", orderItem.NavigatePaths).Should().Be(field);
            orderItem.OrderType.Should().Be(orderType);
        }

        [Fact]
        public void ShouldCanConvertToString()
        {
            var converter = TypeDescriptor.GetConverter(typeof(OrderInfo));
            converter.CanConvertTo(typeof(string)).Should().Be(true);
            OrderInfo orderItem = OrderInfo.Parse("Field.Desc()");
            converter.ConvertTo(orderItem, typeof(string)).Should().Be("Field.desc()");
        }
        [Fact]
        public void ShouldCanConvertFromString()
        {
            var converter = TypeDescriptor.GetConverter(typeof(OrderInfo));
            converter.CanConvertFrom(typeof(string)).Should().Be(true);
            converter.ConvertFrom("Field.desc(),Field2.desc()").Should().BeOfType<OrderInfo>();
        }
    }
}
