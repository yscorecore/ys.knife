
using System;
using System.Linq.Expressions;

namespace YS.Knife.Data.Query.Expressions
{

    public interface IValueLambdaProvider
    {
        Type SourceType { get; }
        LambdaExpression GetLambda(ParameterExpression parameterExpression);
        LambdaExpression GetLambda(ParameterExpression parameterExpression, Type targetType);
        LambdaExpression GetLambda()
        {
            var parameter = Expression.Parameter(SourceType, "p");
            return GetLambda(parameter);
        }
        public LambdaExpression GetLambda(Type targetType)
        {
            var parameter = Expression.Parameter(SourceType, "p");
            return GetLambda(parameter, targetType);
        }
    }
}
