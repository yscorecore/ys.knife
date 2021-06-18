using System;
using System.Linq.Expressions;

namespace YS.Knife.Data.Mappers
{
    class FromNullablePropertyMapperExpression<TValue> : MapperExpression<TValue,TValue>
        where TValue:struct
    {


        public FromNullablePropertyMapperExpression(LambdaExpression sourceExpression)
        :base(sourceExpression)
        {
        }

        public override bool IsCollection { get=>false; }

        public override LambdaExpression GetBindExpression()
        {
            var body = this.SourceExpression.Body;
            var expression = Expression.Convert(body, typeof(Nullable<>).MakeGenericType(typeof(TValue)));
            return Expression.Lambda(expression, this.SourceExpression.Parameters);
        }

       

        public static FromNullablePropertyMapperExpression<TValue> Create<TSource>(Expression<Func<TSource, TValue>> sourceExpression)
            where TSource:class
        {
            return new FromNullablePropertyMapperExpression<TValue>(sourceExpression);
        }
    }

}
