using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Data
{
    class OrderInfoParser
    {
        public static OrderInfoParser Default = new OrderInfoParser();
        public OrderInfo ParseOrderInfo(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            var context = new ParseContext(text);
            OrderInfo orderInfo = ParseOrderInfo(context);
            context.SkipWhiteSpace();
            if (context.NotEnd())
            {
                throw ParseErrors.InvalidText(context);
            }
            return orderInfo;
        }

        internal OrderInfo ParseOrderInfo(ParseContext context)
        {
            OrderInfo orderInfo = new OrderInfo();
            while (context.SkipWhiteSpace())
            {
                var (success, name) = context.TryParseName();
                if (!success)
                {
                    break;
                }
                orderInfo.Add(OrderItem.Parse(name));

                if (context.SkipWhiteSpace() && context.Current() == ',')
                {
                    context.Index++;
                }
                else
                {
                    break;
                }
            }
            return orderInfo;
        }
    }
}
