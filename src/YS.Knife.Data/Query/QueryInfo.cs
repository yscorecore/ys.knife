using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace YS.Knife.Data.Query
{
    [Serializable]
    [DebuggerDisplay("{ToString()}")]
    public record QueryInfo
    {
        [MaxLength(4096)]
        public string Filter { get; set; }
        [MaxLength(1024)]
        public string OrderBy { get; set; }
        [MaxLength(1024)]
        public string Select { get; set; }
    }
    [Serializable]
    [DebuggerDisplay("{ToString()}")]
    public record LimitQueryInfo : QueryInfo, ILimitInfo
    {
        [Range(0, int.MaxValue)]
        public int Offset { get; set; } = 0;
        [Range(1, 10000)]
        public int Limit { get; set; } = 10000;
    }
}
