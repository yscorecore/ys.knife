using System;
using System.Linq.Expressions;

namespace YS.Knife.Data.Query.Expressions
{
    public class PathValueLambdaProvider<TSource> : IFuncLambdaProvider
    {
        public Type SourceType => typeof(TSource);
        public LambdaExpression GetLambda()
        {
            throw new NotImplementedException();
        }
        public LambdaExpression GetLambda(Type targetType)
        {
            return null;
        }
    }
}