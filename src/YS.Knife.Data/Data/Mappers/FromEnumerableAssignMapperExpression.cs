using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace YS.Knife.Data.Mappers
{
    class FromEnumerableAssignMapperExpression<TSourceValueItem,TTargetValueItem> : MapperExpression

        where TSourceValueItem : TTargetValueItem

    {
        public override bool IsCollection { get => true; }


        public FromEnumerableAssignMapperExpression(LambdaExpression sourceExpression, Type targetValueCollection):base(sourceExpression,sourceExpression.ReturnType,targetValueCollection)
        {
            
        }
        public override LambdaExpression GetBindExpression()
        {
            bool sourceIsQueryable = EnumerableTypeUtils.IsQueryable(SourceValueType);
            bool targetIsQueryable = EnumerableTypeUtils.IsQueryable(TargetValueType);
            var selectMethod = sourceIsQueryable? MethodFinder.GetQuerybleSelect<TSourceValueItem, TTargetValueItem>():MethodFinder.GetEnumerableSelect<TSourceValueItem, TTargetValueItem>();
            var callSelectExpression = Expression.Call(selectMethod, this.SourceExpression.Body, GetAssignExpression());
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
                    MethodFinder.GetAsQueryable(EnumerableTypeUtils.GetQueryableItemType(TargetValueType)),
                    selectExpression);
            }
          
            var toResultMethod = this.TargetValueType.IsArray ? MethodFinder.GetEnumerableToArray<TTargetValueItem>() : MethodFinder.GetEnumerableToList<TTargetValueItem>();
               
            return Expression.Call(toResultMethod, selectExpression);

        }




        private Expression<Func<TSourceValueItem, TTargetValueItem>> GetAssignExpression()
        {
            var p = Expression.Parameter(typeof(TSourceValueItem));
            return Expression.Lambda<Func<TSourceValueItem, TTargetValueItem>>(p, p);
        }

        
    }

    
}
