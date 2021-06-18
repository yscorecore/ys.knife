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

        public FromNewComplexObjectMapperExpression(LambdaExpression sourceExpression, ObjectMapper<TSourceValue, TTargetValue> objectMapper)
            :base(sourceExpression)
        {
            
            this.SubMapper = objectMapper??throw new ArgumentNullException(nameof(objectMapper));
        }

        public override bool IsCollection { get=>false; }


        public override LambdaExpression GetBindExpression()
        {
            var newObjectExpression = this.SubMapper.BuildExpression();
            var expression = newObjectExpression.ReplaceFirstParam(this.SourceExpression.Body);
            // 不需要处理source为null的情况，submapper 已经处理
            return Expression.Lambda(expression, this.SourceExpression.Parameters.First());
        }



        public static FromNewComplexObjectMapperExpression<TSourceValue, TTargetValue> Create<TSource>(Expression<Func<TSource, TSourceValue>> sourceExpression,ObjectMapper<TSourceValue, TTargetValue> objectMapper)
            where TSource:class
        {
            return new FromNewComplexObjectMapperExpression<TSourceValue, TTargetValue>(sourceExpression,objectMapper);
        }
    }
}
