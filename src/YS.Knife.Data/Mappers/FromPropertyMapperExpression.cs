using System;
using System.Linq.Expressions;

namespace YS.Knife.Data.Mappers
{
    class FromPropertyMapperExpression<TSourceValue, TTargetValue> : MapperExpression<TSourceValue, TTargetValue>
        where TSourceValue : TTargetValue
    {

        public FromPropertyMapperExpression(LambdaExpression sourceExpression):base(sourceExpression)
        {

        }


        public override bool IsCollection
        {
            get => SourceValueType != typeof(string) && TargetValueType != typeof(string) &&
                   SourceValueType.IsEnumerable() &&
                   TargetValueType.IsEnumerable();
        }

        public override LambdaExpression GetBindExpression()
        {
            return this.SourceExpression;
        }

       
        public static FromPropertyMapperExpression<TSourceValue, TTargetValue> Create<TSource>(
            Expression<Func<TSource, TSourceValue>> sourceExpression)
            where TSource : class
        {
            return new FromPropertyMapperExpression<TSourceValue, TTargetValue>(sourceExpression);
        }
    }
}
