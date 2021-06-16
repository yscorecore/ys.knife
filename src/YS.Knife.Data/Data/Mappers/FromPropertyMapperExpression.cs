using System;
using System.Linq.Expressions;

namespace YS.Knife.Data.Mappers
{
    class FromPropertyMapperExpression<TSourceValue, TTargetValue> : MapperExpression<TSourceValue, TTargetValue>
        where TSourceValue : TTargetValue
    {
        private readonly LambdaExpression sourceExpression;

        public FromPropertyMapperExpression(LambdaExpression sourceExpression)
        {
            _ = sourceExpression ?? throw new ArgumentNullException(nameof(sourceExpression));
            this.sourceExpression = sourceExpression;
        }


        public override bool IsCollection
        {
            get => SourceValueType != typeof(string) && TargetValueType != typeof(string) &&
                   EnumerableTypeUtils.IsEnumerable(SourceValueType) &&
                   EnumerableTypeUtils.IsEnumerable(TargetValueType);
        }

        public override LambdaExpression GetLambdaExpression()
        {
            return this.sourceExpression;
        }

        public static FromPropertyMapperExpression<TSourceValue, TTargetValue> Create<TSource>(
            Expression<Func<TSource, TSourceValue>> sourceExpression)
            where TSource : class
        {
            return new FromPropertyMapperExpression<TSourceValue, TTargetValue>(sourceExpression);
        }
    }
}
