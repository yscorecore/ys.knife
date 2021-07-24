using System;
using System.Collections.Generic;
using System.Linq;
using YS.Knife.Data.Query.Functions.Order;

namespace YS.Knife.Data.Query
{
    public class OrderItem
    {
        public List<ValuePath> NavigatePaths { get; set; }
        public OrderType OrderType { get; set; }

        public override string ToString()
        {
            var paths = NavigatePaths.TrimNotNull();
            if (paths.Count() > 0)
            {
                string path = string.Join(".", paths);
                return OrderType == OrderType.Desc ? $"{path}.desc()" : $"{path}.asc()";
            }
            return string.Empty;
        }
        internal static OrderItem FromValuePaths(List<ValuePath> paths)
        {
            var last = paths?.LastOrDefault();

            if (last != null && last.IsFunction)
            {
                if (string.Equals(last.Name, nameof(Desc), StringComparison.InvariantCultureIgnoreCase))
                {
                    return new OrderItem
                    {
                        NavigatePaths = paths.SkipLast(1).ToList(),
                        OrderType = OrderType.Desc
                    };
                }
                else if (string.Equals(last.Name, nameof(Asc), StringComparison.InvariantCultureIgnoreCase))
                {
                    return new OrderItem
                    {
                        NavigatePaths = paths.SkipLast(1).ToList(),
                        OrderType = OrderType.Asc
                    };
                }
            }
            return new OrderItem
            {
                NavigatePaths = paths,
                OrderType = OrderType.Asc
            };
        }
    }
}
