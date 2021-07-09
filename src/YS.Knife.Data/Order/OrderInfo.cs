using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace YS.Knife.Data
{
    [TypeConverter(typeof(OrderInfoConverter))]
    [Serializable]
    [DebuggerDisplay("{ToString()}")]
    public class OrderInfo
    {
        public OrderInfo()
        {
        }

        public OrderInfo(IEnumerable<OrderItem> orderItems)
        {
            var items = (orderItems ?? Enumerable.Empty<OrderItem>()).Where(p => p != null);
            this.Items.AddRange(items);
        }

        public List<OrderItem> Items { get; private set; } = new List<OrderItem>();



        public static OrderInfo Create(OrderItem orderItem)
        {
            return new OrderInfo(new OrderItem[] { orderItem });
        }
        public static OrderInfo Create(string name, OrderType orderType = OrderType.Asc)
        {
            return Create(new OrderItem { FieldName = name, OrderType = orderType });
        }
        public static OrderInfo Create(params OrderItem[] orderItems)
        {
            return new OrderInfo(orderItems);
        }
        public static OrderInfo Parse(string orderText)
        {
            return OrderInfoParser.Default.ParseOrderInfo(orderText);
        }


        public bool HasItems()
        {
            return this.Items != null && this.Items.Count > 0;
        }

        public override string ToString()
        {
            return string.Join(",", (this.Items ?? Enumerable.Empty<OrderItem>()).Where(item => item != null));
        }

        public OrderInfo Add(OrderItem orderItem)
        {
            this.Items.Add(orderItem);
            return this;
        }
        public OrderInfo Add(string fieldName, OrderType orderType)
        {
            return this.Add(new OrderItem(fieldName, orderType));
        }
    }

    public class OrderInfoConverter : StringConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return (value is string orderText) ? OrderInfo.Parse(orderText) : base.ConvertFrom(context, culture, value);
        }

    }
}
