using System;
using System.Diagnostics;
using YS.Knife.Data.Expressions;

namespace YS.Knife.Data
{
    [Serializable]
    [DebuggerDisplay("{ToString()}")]
    public class QueryInfo
    {
        public string Filter { get; set; }
        public string OrderBy { get; set; }
        public string Select { get; set; }
    }

    public class PageQueryInfo : QueryInfo, ILimitInfo
    {
        //[DataAnnotations.Range(0, 10)]
        public int Offset { get; set; }

        public int Limit { get; set; }
    }
    public class SliceQueryInfo<T> : QueryInfo, ISliceInfo
    {
        public string Start { get; set; }

        public int Limit { get; set; }
    }
}
