using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace YS.Knife.Data.Mappers
{
    class FromQueryableNewObjectMapperExpression<TSourceValueCollection,TSourceValueItem,TTargetValueCollection,TTargetValueItem> : MapperExpression<TSourceValueCollection,TTargetValueCollection>

        where TTargetValueCollection:IEnumerable<TTargetValueItem>
        where TSourceValueCollection:IEnumerable <TSourceValueItem>
        where TTargetValueItem : class, new()
        where TSourceValueItem : class
  
    {

        public override bool IsCollection { get => true; }

        public FromQueryableNewObjectMapperExpression(LambdaExpression sourceExpression, ObjectMapper<TSourceValueItem, TTargetValueItem> objectMapper):base(sourceExpression)
        {
            this.SubMapper = objectMapper;
        }

        public override LambdaExpression GetBindExpression()
        {
            var newObjectExpression = this.SubMapper.BuildExpression();

            var selectMethod = EnumerableTypeUtils.IsQueryable(SourceValueType)? MethodFinder.GetQuerybleSelect<TSourceValueItem, TTargetValueItem>():MethodFinder.GetEnumerableSelect<TSourceValueItem, TTargetValueItem>();
            var toResultMethod = this.TargetValueType.IsArray ? MethodFinder.GetEnumerableToArray<TTargetValueItem>() : MethodFinder.GetEnumerableToList<TTargetValueItem>();
            var callSelectExpression = Expression.Call(selectMethod, this.SourceExpression.Body, newObjectExpression);
            var toResultExpression = Expression.Call(toResultMethod, callSelectExpression);
            // 需要处理source为null的情况
            var resultExpression = Expression.Condition(
                 Expression.Equal(this.SourceExpression.Body, Expression.Constant(null))
                , Expression.Constant(null, this.TargetValueType), toResultExpression);
            return Expression.Lambda(resultExpression, this.SourceExpression.Parameters.First());

        }
        
        public static FromQueryableNewObjectMapperExpression<TSourceValueCollection,TSourceValueItem,TTargetValueCollection,TTargetValueItem> Create<TSource>(Expression<Func<TSource, TSourceValueCollection>> sourceExpression,ObjectMapper<TSourceValueItem, TTargetValueItem> objectMapper)
            where TSource:class
        {
            return new FromQueryableNewObjectMapperExpression<TSourceValueCollection,TSourceValueItem,TTargetValueCollection,TTargetValueItem>(sourceExpression,objectMapper);
        }

    }
}
