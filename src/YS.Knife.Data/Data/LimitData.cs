using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace YS.Knife.Data
{
    [Serializable]
    public class LimitData<TData> : ILimitData<TData>
    {
        public LimitData()
        {
        }
        public LimitData(IEnumerable<TData> limitListData, int offset, int limit, int totalCount)
        {
            this.Limit = limit;
            this.Offset = offset;
            this.TotalCount = totalCount;
            this.ListData = (limitListData ?? Enumerable.Empty<TData>()).ToList();
        }


        public bool HasNext
        {
            get
            {
                return this.TotalCount > this.Offset + this.Limit;
            }
        }

        public int Limit { get; set; }


        public List<TData> ListData { get; set; }

        public int Offset { get; set; }


        public int TotalCount { get; set; }

    }
}
