using System;
using System.Linq.Expressions;
using YS.Knife.Data.Filter;
using YS.Knife.Data.Query.Expressions;
using static YS.Knife.Data.Filter.FilterInfoExpressionBuilder;

namespace YS.Knife.Data.Query.Functions
{
    public class FunctionResult
    {
        public LambdaExpression LambdaExpression { get; set; }
        public Type LambdaValueType { get; set; }

        public IMemberVisitor MemberProvider { get; set; }

    }
}
