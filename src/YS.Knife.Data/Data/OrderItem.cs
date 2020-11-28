using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace YS.Knife.Data
{
    [TypeConverter(typeof(OrderItemConverter))]
    [Serializable]
    public class OrderItem
    {
        public OrderItem()
        {
        }
        public OrderItem(string fieldName, OrderType orderType = OrderType.Asc)
        {
            _ = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
            this.FieldName = fieldName;
            this.OrderType = orderType;
        }

        public string FieldName { get; set; }
        public OrderType OrderType { get; set; }

        public override string ToString()
        {
            var op = OrderType == OrderType.Asc ? "+" : "-";
            return $"{op}{FieldName}";
        }
        public static OrderItem Parse(string orderStr)
        {
            _ = orderStr ?? throw new ArgumentNullException(nameof(orderStr));
            var chars = orderStr.ToCharArray();
            var orderType = OrderType.Asc;
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '+')
                {
                    orderType = OrderType.Asc;
                }
                else if (chars[i] == '-')
                {
                    orderType = OrderType.Desc;
                }
                else
                {
                    return new OrderItem(orderStr.Substring(i), orderType);
                }
            }
            throw new FormatException("Invalid order item text, missing field name.");
        }
    }

    public class OrderItemConverter : StringConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string orderItemText)
            {
                return OrderItem.Parse(orderItemText);
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
