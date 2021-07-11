using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

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

            //scores{a=b,+a,-b,1,3}(a,b)
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Name);
            if (CollectionFilter != null || CollectionOrder != null || CollectionLimit != null)
            {
                sb.Append("{");
                string collectionInfo = string.Join(',',
                    new string[] {
                        CollectionLimit?.ToString(),
                        CollectionOrder?.ToString(),
                        CollectionFilter?.ToString(),
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
    }
}
