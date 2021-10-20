using System;
using System.ComponentModel;
using System.Linq.Expressions;
using YS.Knife.Data.Query;

namespace YS.Knife.Data.Query.Expressions
{
    internal class ConstValueLambdaProvider : IValueLambdaProvider
    {
        public ConstValueLambdaProvider(Type sourceType, object constValue)
        {
            this.SourceType = sourceType;
            this.ConstValue = constValue;
        }

        public object ConstValue { get; }

        public Type SourceType { get; }

        public LambdaExpression GetLambda(ParameterExpression parameter)
        {
            var targetType = ConstValue?.GetType() ?? typeof(string);
            var constExp = Expression.Constant(this.ConstValue, targetType);
            return Expression.Lambda(typeof(Func<,>).MakeGenericType(SourceType, targetType), constExp, parameter);
        }

        public LambdaExpression GetLambda(ParameterExpression parameter, Type targetType)
        {
            _ = targetType ?? throw new ArgumentNullException(nameof(targetType));
            var constExp = Expression.Constant(ValueConverter.Instance.ChangeType(this.ConstValue, targetType), targetType);
            return Expression.Lambda(typeof(Func<,>).MakeGenericType(SourceType, targetType), constExp, parameter);
        }
    }
}
