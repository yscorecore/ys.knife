using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace YS.Knife.Data.Mappers
{
    class FromEnumerableNewObjectMapperExpression<TSourceValueItem,TTargetValueItem> : MapperExpression

        where TTargetValueItem : class, new()
        where TSourceValueItem : class
  
    {

        public override bool IsCollection { get => true; }

        public FromEnumerableNewObjectMapperExpression(LambdaExpression sourceExpression, Type targetValueCollection, ObjectMapper<TSourceValueItem, TTargetValueItem> objectMapper)
            :base(sourceExpression,sourceExpression.ReturnType,targetValueCollection)
        {
            this.SubMapper = objectMapper;
        }

        public override LambdaExpression GetBindExpression()
        {
            bool sourceIsQueryable = SourceValueType.IsQueryable();
            bool targetIsQueryable = TargetValueType.IsQueryable();
            var newObjectExpression = this.SubMapper.BuildExpression();
            var selectMethod = sourceIsQueryable? MethodFinder.GetQuerybleSelect<TSourceValueItem, TTargetValueItem>():MethodFinder.GetEnumerableSelect<TSourceValueItem, TTargetValueItem>();
            var callSelectExpression = Expression.Call(selectMethod, this.SourceExpression.Body, newObjectExpression);
            var toResultExpression = GetResultExpression(callSelectExpression, sourceIsQueryable, targetIsQueryable);
            // 需要处理source为null的情况
            var resultExpression = Expression.Condition(
                 Expression.Equal(this.SourceExpression.Body, Expression.Constant(null))
                , Expression.Constant(null, this.TargetValueType), toResultExpression);
            return Expression.Lambda(resultExpression, this.SourceExpression.Parameters.First());

        }
        
        private Expression GetResultExpression(Expression selectExpression,bool sourceIsQueryable, bool targetIsQueryable)
        {
            if (sourceIsQueryable && targetIsQueryable )
            {
                return selectExpression;
            }

            if (!sourceIsQueryable && targetIsQueryable)
            {
                return Expression.Call(
                    MethodFinder.GetAsQueryable(TargetValueType.GetQueryableItemType()),
                    selectExpression);
            }
          
            var toResultMethod = this.TargetValueType.IsArray ? MethodFinder.GetEnumerableToArray<TTargetValueItem>() : MethodFinder.GetEnumerableToList<TTargetValueItem>();
               
            return Expression.Call(toResultMethod, selectExpression);

        }


    }
}
