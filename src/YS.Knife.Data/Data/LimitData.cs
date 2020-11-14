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
        public LimitData(IQueryable<TData> source, int offset, int limit)
        {
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit));
            this.Limit = limit;
            this.Offset = offset;
            this.TotalCount = source.Count();
            this.lst.AddRange(source.Skip(offset).Take(limit).ToList());
        }
        public LimitData(IList<TData> source, int offset, int limit)
        {
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit));
            this.Limit = limit;
            this.Offset = offset;
            this.TotalCount = source.Count;
            this.lst.AddRange(source.Skip(offset).Take(limit).ToList());
        }
        public LimitData(IEnumerable<TData> listData, int offset, int limit,int totalCount)
        {
            this.Limit = limit;
            this.Offset = offset;
            this.TotalCount = totalCount;
            this.lst.AddRange(ListData);
        }
        public bool ContainsListCollection
        {
            get
            {
                return true;
            }
        }

        public bool HasNext
        {
            get
            {
                return this.TotalCount > this.Offset + this.Limit;
            }
        }

        public int Limit { get; set; }

        private readonly List<TData> lst = new List<TData>();
        public List<TData> ListData
        {
            get
            {
                return this.lst;
            }
        }

        public int Offset { get; set; }
       

        public int TotalCount { get; set; }
        

        public IList GetList()
        {
            return this.lst;
        }
    }
}
