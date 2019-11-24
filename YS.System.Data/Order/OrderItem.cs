using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace System.Data
{
    [System.ComponentModel.TypeConverter(typeof(OrderItem))]
    [Serializable]
    public class OrderItem
    {
        /// <summary>
        /// 隐式转化为OrderCondition
        /// </summary>
        /// <param name="orderItem"></param>
        public static implicit operator OrderCondition(OrderItem orderItem)
        {
            if (orderItem == null) return new OrderCondition();
            return new OrderCondition(new OrderItem[] { orderItem });
            //return orderItem.AsCondition();
        }
        public string FieldName { get; set; }
        public OrderType OrderType { get; set; }

        public override string ToString()
        {
            var op = OrderType == OrderType.ASC ? string.Empty : "-";
            return $"{op}{FieldName}";
        }
        public static OrderItem Parse(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }
            text = text.Trim();
            if (text.StartsWith("+"))
            {
                return new OrderItem()
                {
                    FieldName = text.Substring(1),
                    OrderType = OrderType.ASC
                };
            }
            else if (text.StartsWith("-"))
            {
                return new OrderItem()
                {
                    FieldName = text.Substring(1),
                    OrderType = OrderType.DESC
                };
            }
            else
            {
                return new OrderItem()
                {
                     FieldName= text,
                      OrderType= OrderType.ASC
                };
            }
        }
    }

    public class OrderItemConverter:StringConverter
    {

    }
}
