using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Data.Query
{
    [Serializable]
    public class LimitList<TData> : ILimitList<TData>
    {
        public LimitList()
        {

        }
        public LimitList(IEnumerable<TData> items, int offset, int limit)
        {
            this.Items = (items ?? Enumerable.Empty<TData>()).ToList();
            this.Offset = offset;
            this.Limit = limit;
        }
        public List<TData> Items { get; set; }

        public int Offset { get; set; }

        public int Limit { get; set; }

        public bool HasNext { get => Items?.Count >= Limit; }

    }
}
