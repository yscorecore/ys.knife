using System;
using System.ComponentModel;
using System.Globalization;

namespace YS.Knife.Data
{
    [Serializable]
    public class OrderItem
    {
        public OrderItem()
        {

        }
        public OrderItem(string fieldName, OrderType orderType)
        {
            this.FieldName = fieldName;
            this.OrderType = orderType;
        }
        public string FieldName { get; set; }
        public OrderType OrderType { get; set; }

        public override string ToString()
        {
            return $"{FieldName}__{OrderType.ToString().ToLowerInvariant()}";
        }
        public static OrderItem Parse(string orderStr)
        {
            _ = orderStr ?? throw new ArgumentNullException(nameof(orderStr));
            var trimedOrderText = orderStr.Trim();
            if (trimedOrderText.EndsWith("__asc",StringComparison.InvariantCultureIgnoreCase))
            {
                return new OrderItem { FieldName = trimedOrderText[..^5], OrderType = OrderType.Asc };
            }
            else if (trimedOrderText.EndsWith("__desc", StringComparison.InvariantCultureIgnoreCase))
            {
                return new OrderItem { FieldName = trimedOrderText[..^6] , OrderType = OrderType.Desc };
            }
            else {
                return new OrderItem { FieldName = trimedOrderText, OrderType = OrderType.Asc };
            }
           
          
        }
    }

}
