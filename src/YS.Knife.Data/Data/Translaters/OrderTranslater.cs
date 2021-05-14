using System;
using System.Linq;

namespace YS.Knife.Data.Translaters
{
    public class OrderTranslater
    {
        public OrderItem Translate<TFrom, TTo>(OrderItem orderItem)
        {
            return Translate(typeof(TFrom), typeof(TTo), orderItem);
        }
        public OrderInfo Translate(Type from, Type to, OrderInfo orderInfo)
        {
            return OrderInfo.Create(orderInfo.Items.TrimNotNull().Select(p => Translate(from, to, p)).ToArray());
        }
        
        public OrderInfo Translate<TFrom, TTo>(OrderInfo orderInfo)
        {
            return Translate(typeof(TFrom), typeof(TTo), orderInfo);
        }
        
        public OrderItem Translate(Type from, Type to, OrderItem orderItem)
        {
            return null;
        }
    }
}
