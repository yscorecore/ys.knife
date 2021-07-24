using System;
using System.Collections.Generic;
using System.Linq;

namespace YS.Knife.Data.Query
{
    [Serializable]
    public class PagedList<TData> : IPagedList<TData>
    {
        public PagedList()
        {
        }
        public PagedList(IEnumerable<TData> limitListData, int offset, int limit, int totalCount)
        {
            this.Limit = limit;
            this.Offset = offset;
            this.TotalCount = totalCount;
            this.Items = (limitListData ?? Enumerable.Empty<TData>()).ToList();
        }


        public bool HasNext
        {
            get
            {
                return this.TotalCount > this.Offset + this.Limit;
            }
        }

        public int Limit { get; set; }


        public List<TData> Items { get; set; }

        public int Offset { get; set; }


        public int TotalCount { get; set; }

    }
}
