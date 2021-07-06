using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using static YS.Knife.Data.Filter.FilterInfoExpressionBuilder;

namespace YS.Knife.Data.Filter.Functions
{
    public class FunctionContext
    {
        public string Name { get; set; }
        // subfilter and sub mapper only for collection type
        public FilterInfo2 SubFilter { get; set; }
        public List<FilterValue> Args { get; set; }
        public Expression CurrentExpression { get; set; }
        public IMemberExpressionProvider MemberExpressionProvider { get; set; }
    }
}
