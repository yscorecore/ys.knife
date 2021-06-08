using System;
using System.Linq.Expressions;

namespace YS.Knife.Data.Mappers
{
    class FromNullablePropertyMapperExpression<TValue> : MapperExpression<TValue,TValue>
        where TValue:struct
    {

        private readonly LambdaExpression sourceExpression;

        public FromNullablePropertyMapperExpression(LambdaExpression sourceExpression)
        {
            this.sourceExpression = sourceExpression;
        }

        public override bool IsCollection { get=>false; }

        public override LambdaExpression GetLambdaExpression()
        {
            var body = this.sourceExpression.Body;
            var expression = Expression.Convert(body, typeof(Nullable<>).MakeGenericType(typeof(TValue)));
            return Expression.Lambda(expression, this.sourceExpression.Parameters);
        }

        public static FromNullablePropertyMapperExpression<TValue> Create<TSource>(Expression<Func<TSource, TValue>> sourceExpression)
            where TSource:class
        {
            return new FromNullablePropertyMapperExpression<TValue>(sourceExpression);
        }
    }

}
