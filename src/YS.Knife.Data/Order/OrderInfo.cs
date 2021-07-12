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

        public OrderInfo(params OrderItem2[] orderItems)
        {
            var items = (orderItems ?? Enumerable.Empty<OrderItem2>()).Where(p => p != null);
            this.Items.AddRange(items);
        }

        public List<OrderItem2> Items { get; private set; } = new List<OrderItem2>();



        public static OrderInfo Create(OrderItem2 orderItem)
        {
            return new OrderInfo(new OrderItem2[] { orderItem });
        }
        public static OrderInfo Create(string fieldNames, OrderType orderType = OrderType.Asc)
        {
            var orderInfo = new OrderInfo();

            return orderInfo.Add(fieldNames, orderType);
        }
        public static OrderInfo Create(params OrderItem2[] orderItems)
        {
            return new OrderInfo(orderItems);
        }
        public static OrderInfo Parse(string orderText)
        {
            return Parse(orderText, CultureInfo.CurrentCulture);
        }
        public static OrderInfo Parse(string orderText,CultureInfo cultureInfo)
        {
            return  new FilterInfoParser2(cultureInfo).ParseOrder(orderText);
        }

        public bool HasItems()
        {
            return this.Items != null && this.Items.Count > 0;
        }

        public override string ToString()
        {
            return string.Join(",", (this.Items ?? Enumerable.Empty<OrderItem2>()).Where(item => item != null));
        }

        public OrderInfo Add(OrderItem2 orderItem)
        {
            this.Items.Add(orderItem);
            return this;
        }
        public OrderInfo Add(string fieldPaths, OrderType orderType)
        {
            return this.Add(OrderItem2.Create(fieldPaths,orderType));
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
