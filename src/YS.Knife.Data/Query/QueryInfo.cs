using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using YS.Knife.Data.Query;

namespace YS.Knife.Data
{
    [Serializable]
    [DebuggerDisplay("{ToString()}")]
    public record QueryInfo
    {
        public string Filter { get; set; }
        public string OrderBy { get; set; }
        public string Select { get; set; }
    }

    public record PagedQueryInfo : QueryInfo, ILimitInfo
    {
        [Range(0, int.MaxValue)]
        public int Offset { get; set; } = 0;
        [Range(1, 10000)]
        public int Limit { get; set; } = 10000;
    }
}
