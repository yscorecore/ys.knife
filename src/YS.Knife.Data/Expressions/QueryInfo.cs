using System;
using System.Diagnostics;

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

    public class PageQueryInfo:QueryInfo
    {
        public string Limit { get; set; }
    }


}
