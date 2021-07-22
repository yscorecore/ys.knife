using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using YS.Knife.Data.Query;

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
        [Range(0, int.MaxValue)]
        public int Offset { get; set; }
        [Range(1, 10000)]
        public int Limit { get; set; }
    }
    public class SliceQueryInfo<T> : QueryInfo, ISliceInfo
    {
        public string Start { get; set; }
        [Range(1, 10000)]
        public int Limit { get; set; }
    }
}
