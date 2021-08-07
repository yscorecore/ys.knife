
using System;
using System.Linq.Expressions;

namespace YS.Knife.Data.Query.Expressions
{

    public interface IValueLambdaProvider
    {
        Type SourceType { get; }
        LambdaExpression GetLambda();
        LambdaExpression GetLambda(Type targetType);
    }
}
