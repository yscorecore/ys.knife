using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace YS.Knife.Data.Mappers
{

    
    // class FromEnumerableAssignMapperExpression<TSource, TSourceValue, TTargetValue> : MapperExpression<IEnumerable<TSourceValue>,IEnumerable<TTargetValue>>
    //  where TSourceValue : TTargetValue
    // {
    //     private readonly Type resultType;
    //     private readonly Expression<Func<TSource, IEnumerable<TSourceValue>>> sourceExpression;
    //     public override bool IsCollection { get => true; }
    //
    //     public FromEnumerableAssignMapperExpression(Expression<Func<TSource, IEnumerable<TSourceValue>>> sourceExpression, Type resultType)
    //     {
    //         this.sourceExpression = sourceExpression;
    //         this.resultType = resultType;
    //     }
    //     public override LambdaExpression GetLambdaExpression()
    //     {
    //         var selectMethod = MethodFinder.GetEnumerableSelect<TSourceValue, TTargetValue>();
    //         var toResultMethod = resultType.IsArray ? MethodFinder.GetEnumerableToArray<TTargetValue>() : MethodFinder.GetEnumerableToList<TTargetValue>();
    //         var callSelectExpression = Expression.Call(selectMethod, this.sourceExpression.Body, GetAssignExpression());
    //         var toResultExpression = Expression.Call(toResultMethod, callSelectExpression);
    //         // 需要处理source为null的情况
    //         var resultExpression = Expression.Condition(
    //              Expression.Equal(this.sourceExpression.Body, Expression.Constant(null))
    //             , Expression.Constant(null, resultType), toResultExpression);
    //         return Expression.Lambda(resultExpression, this.sourceExpression.Parameters.First());
    //
    //     }
    //
    //
    //     private Expression<Func<TSourceValue, TTargetValue>> GetAssignExpression()
    //     {
    //         var p = Expression.Parameter(typeof(TSourceValue));
    //         return Expression.Lambda<Func<TSourceValue, TTargetValue>>(p, p);
    //     }
    // }
}
