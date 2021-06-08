using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;

namespace YS.Knife.Data.Mappers
{
    
    class FromNewComplexObjectMapperExpression<TSourceValue, TTargetValue>: MapperExpression<TSourceValue,TTargetValue>
        where TSourceValue : class
        where TTargetValue : class, new()
    {
        private readonly LambdaExpression sourceExpression;

        public FromNewComplexObjectMapperExpression(LambdaExpression sourceExpression, ObjectMapper<TSourceValue, TTargetValue> objectMapper)
        {
            
            this.SubMapper = objectMapper??throw new ArgumentNullException(nameof(objectMapper));
            this.sourceExpression = sourceExpression;
        }

        public override bool IsCollection { get=>false; }


        public override LambdaExpression GetLambdaExpression()
        {
            var newObjectExpression = this.SubMapper.BuildLambdaExpression();
            var expression = newObjectExpression.ReplaceFirstParam(this.sourceExpression.Body);
            // 需要处理source为null的情况
            var resultExpression = Expression.Condition(
                 Expression.Equal(this.sourceExpression.Body, Expression.Constant(null))
                ,Expression.Constant(null,typeof(TTargetValue)), expression);

            return Expression.Lambda(resultExpression, this.sourceExpression.Parameters.First());
        }

        public static FromNewComplexObjectMapperExpression<TSourceValue, TTargetValue> Create<TSource>(Expression<Func<TSource, TSourceValue>> sourceExpression,ObjectMapper<TSourceValue, TTargetValue> objectMapper)
            where TSource:class
        {
            return new FromNewComplexObjectMapperExpression<TSourceValue, TTargetValue>(sourceExpression,objectMapper);
        }
    }
}
