using System;
using System.Diagnostics;

namespace YS.Knife.Data
{
    [Serializable]
    [DebuggerDisplay("{ToString()}")]
    public class QueryInfo
    {
        public FilterInfo2 Filter { get; set; }
        public OrderInfo Order { get; set; }
        public LimitInfo Limit { get; set; }
        public SelectInfo Select { get; set; }
    }


}
