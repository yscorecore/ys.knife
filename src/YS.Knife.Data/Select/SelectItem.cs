using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using YS.Knife.Data.Expressions.Functions.Collections;

namespace YS.Knife.Data
{
    [DebuggerDisplay("{ToString()}")]
    public class SelectItem
    {
        public string Name { get; set; }

        // SubItems for sub object type or collection type
        public List<SelectItem> SubItems { get; set; }

        // subFilter,Order,Limit only for collection type

        public FilterInfo2 CollectionFilter { get; set; }

        public OrderInfo CollectionOrder { get; set; }

        public LimitInfo CollectionLimit { get; set; }

        public override string ToString()
        {
            //source{where(a=b),orderby(a.asc(),b.desc()),limit(1,3)}(a,b)
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Name);
            if (CollectionFilter != null || CollectionOrder != null || CollectionLimit != null)
            {
                sb.Append("{");
                string collectionInfo = string.Join(',',
                    new string[] {
                        LimitInfoToString(CollectionLimit),
                        OrderInfoToString(CollectionOrder),
                        FilterInfoToString( CollectionFilter),
                }.Where(p => p != null));
                sb.Append(collectionInfo);
                sb.Append("}");
            }
            if (SubItems != null)
            {
                sb.Append($"({string.Join(',', SubItems.Where(p => p != null).Select(p => p.ToString()))})");
            }
            return sb.ToString();
        }
        private string LimitInfoToString(LimitInfo limitInfo)
        {
            return limitInfo != null ? $"{nameof(Limit).ToLower()}({limitInfo})" : null;
        }
        private string OrderInfoToString(OrderInfo orderInfo)
        {
            return orderInfo != null ? $"{nameof(OrderBy).ToLower()}({orderInfo})" : null;
        }
        private string FilterInfoToString(FilterInfo2 filterInfo)
        {
            return filterInfo != null ? $"{nameof(Where).ToLower()}({filterInfo})" : null;
        }
    }
}
