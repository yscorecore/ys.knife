using System;
using System.Linq.Expressions;
using YS.Knife.Data.Query;
using YS.Knife.Data.Query.Expressions;

namespace YS.Knife.Data.Query.Functions
{
    public class FunctionResult
    {
        public LambdaExpression LambdaExpression { get; set; }
        public Type LambdaValueType { get; set; }

        public IMemberVisitor MemberProvider { get; set; }

    }
}
