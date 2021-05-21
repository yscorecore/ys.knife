using System;
using System.Linq.Expressions;

namespace YS.Knife.Data.Mappers
{
    public class ToNullablePropertyMapperExpression<TSource,TValue> : IMapperExpression
        where TValue:struct
    {

        private readonly Expression<Func<TSource,TValue>> sourceExpression;

        public ToNullablePropertyMapperExpression(Expression<Func<TSource, TValue>> sourceExpression)
        {
            this.sourceExpression = sourceExpression;
        }

        public Type SourceValueType => typeof(TValue);

        public LambdaExpression GetLambdaExpression()
        {
            var body = this.sourceExpression.Body;
            var expression = Expression.Convert(body, typeof(Nullable<>).MakeGenericType(typeof(TValue)));
            return Expression.Lambda(expression, this.sourceExpression.Parameters);
        }
    }

}
