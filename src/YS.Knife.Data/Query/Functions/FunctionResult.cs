using System;
using System.Linq.Expressions;
using static YS.Knife.Data.Filter.FilterInfoExpressionBuilder;

namespace YS.Knife.Data.Query.Functions
{
    public class FunctionResult
    {
        public LambdaExpression LambdaExpression { get; set; }
        public Type LambdaValueType { get; set; }

        public IMemberExpressionProvider MemberProvider { get; set; }

    }
}
