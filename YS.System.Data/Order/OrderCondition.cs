using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace System.Data
{
    [System.ComponentModel.TypeConverter(typeof(OrderConditionConverter))]
    [Serializable]
    public class OrderCondition
    {
        public List<OrderItem> Items { get;  set; } = new List<OrderItem>();

        public OrderCondition()
        {

        }

        public bool HasItems()
        {
            return this.Items != null && this.Items.Count > 0;
        }
        public OrderCondition(IEnumerable<OrderItem> orderItems)
        {
            this.Items.AddRange(orderItems??Enumerable.Empty<OrderItem>());
        }

        public static OrderCondition Parse(string orderString)
        {
            return new OrderCondition(ParseItems(orderString));
        }

        public static IEnumerable<OrderItem> ParseItems(string orderString)
        {
            int startIndex = 0;
            bool hasData = false;
            for (int i = 0; i < orderString.Length; i++)
            {
                char ch = orderString[i];
                if (ch == '+' || ch == '-')
                {
                    if (i > startIndex + 1)
                    {
                        yield return OrderItem.Parse(orderString.Substring(startIndex, i - startIndex));
                        hasData = false;
                    }
                    startIndex = i;
                }
                else
                {
                    hasData = true;
                }
            }
            if (hasData)
                yield return OrderItem.Parse(orderString.Substring(startIndex));
        }
    }

    public class OrderConditionConverter:TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return  base.CanConvertFrom(context, sourceType)||sourceType==typeof(string) ;
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return base.CanConvertTo(context, destinationType)|| destinationType==typeof(string);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                return OrderCondition.Parse(value as string);
            }
            return base.ConvertFrom(context, culture, value);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var order = value as OrderCondition;
                if (order != null)
                {
                    return order.ToString();
                }
                else
                {
                    return null;
                }
               
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
