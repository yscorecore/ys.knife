using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace YS.Knife.Data.Mappers
{
    class FromQueryableAssignMapperExpression<TSourceValueCollection,TSourceValueItem,TTargetValueCollection,TTargetValueItem> : MapperExpression<TSourceValueCollection,TTargetValueCollection>
    
    
        where TTargetValueCollection:IEnumerable<TTargetValueItem>
        where TSourceValueCollection:IEnumerable<TSourceValueItem>
        where TSourceValueItem : TTargetValueItem

    {
        private readonly LambdaExpression sourceExpression;
        public override bool IsCollection { get => true; }


        public FromQueryableAssignMapperExpression(LambdaExpression sourceExpression)
        {
            this.sourceExpression = sourceExpression;
        }
        public override LambdaExpression GetLambdaExpression()
        {
            var selectMethod = EnumerableTypeUtils.IsQueryable(SourceValueType)? MethodFinder.GetQuerybleSelect<TSourceValueItem, TTargetValueItem>():MethodFinder.GetEnumerableSelect<TSourceValueItem, TTargetValueItem>();
            var toResultMethod = this.TargetValueType.IsArray ? MethodFinder.GetEnumerableToArray<TTargetValueItem>() : MethodFinder.GetEnumerableToList<TTargetValueItem>();
            var callSelectExpression = Expression.Call(selectMethod, this.sourceExpression.Body, GetAssignExpression());
            var toResultExpression = Expression.Call(toResultMethod, callSelectExpression);
            // 需要处理source为null的情况
            var resultExpression = Expression.Condition(
                 Expression.Equal(this.sourceExpression.Body, Expression.Constant(null))
                , Expression.Constant(null, this.TargetValueType), toResultExpression);
            return Expression.Lambda(resultExpression, this.sourceExpression.Parameters.First());

        }


        private Expression<Func<TSourceValueItem, TTargetValueItem>> GetAssignExpression()
        {
            var p = Expression.Parameter(typeof(TSourceValueItem));
            return Expression.Lambda<Func<TSourceValueItem, TTargetValueItem>>(p, p);
        }
        public static FromQueryableAssignMapperExpression<TSourceValueCollection,TSourceValueItem,TTargetValueCollection,TTargetValueItem> Create<TSource>(Expression<Func<TSource, TSourceValueCollection>> sourceExpression)
            where TSource:class
        {
            return new FromQueryableAssignMapperExpression<TSourceValueCollection,TSourceValueItem,TTargetValueCollection,TTargetValueItem>(sourceExpression);
        }
        
    }
    
     class FromEnumerableNullableAssignExpression<TSourceValueCollection,TTargetValueCollection,TValueItem> : MapperExpression<TSourceValueCollection,TTargetValueCollection>
    
    
        where TTargetValueCollection:IEnumerable<TValueItem?>
        where TSourceValueCollection:IEnumerable<TValueItem>
        where TValueItem : struct

    {
        private readonly LambdaExpression sourceExpression;
        public override bool IsCollection { get => true; }


        public FromEnumerableNullableAssignExpression(LambdaExpression sourceExpression)
        {
            this.sourceExpression = sourceExpression;
        }
        public override LambdaExpression GetLambdaExpression()
        {
            var selectMethod = EnumerableTypeUtils.IsQueryable(SourceValueType)? MethodFinder.GetQuerybleSelect<TValueItem, TValueItem?>():MethodFinder.GetEnumerableSelect<TValueItem, TValueItem?>();
            var toResultMethod = this.TargetValueType.IsArray ? MethodFinder.GetEnumerableToArray<TValueItem?>() : MethodFinder.GetEnumerableToList<TValueItem?>();
            var callSelectExpression = Expression.Call(selectMethod, this.sourceExpression.Body, GetAssignExpression());
            var toResultExpression = Expression.Call(toResultMethod, callSelectExpression);
            // 需要处理source为null的情况
            var resultExpression = Expression.Condition(
                 Expression.Equal(this.sourceExpression.Body, Expression.Constant(null))
                , Expression.Constant(null, this.TargetValueType), toResultExpression);
            return Expression.Lambda(resultExpression, this.sourceExpression.Parameters.First());

        }


        private Expression<Func<TValueItem, TValueItem?>> GetAssignExpression()
        {
            var p = Expression.Parameter(typeof(TValueItem));
            var expression = Expression.Convert(p, typeof(TValueItem?));
            return Expression.Lambda<Func<TValueItem, TValueItem?>>(expression, p);
        }
        public static FromEnumerableNullableAssignExpression<TSourceValueCollection,TTargetValueCollection,TValueItem> Create<TSource>(Expression<Func<TSource, TSourceValueCollection>> sourceExpression)
            where TSource:class
        {
            return new FromEnumerableNullableAssignExpression<TSourceValueCollection,TTargetValueCollection,TValueItem>(sourceExpression);
        }
        
    }
    
}
