using System;
using System.Collections.Generic;

namespace YS.Knife.Data.Filter.Functions
{
    public class FunctionContext
    {
        public Type FromType { get; set; }
        // subfilter and sub mapper only for collection type
        public FilterInfo2 SubFilter { get; set; }
        public List<FilterValue> Args { get; set; }
    }
}
