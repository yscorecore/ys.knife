using System;
using System.Linq;
using System.Linq.Expressions;

namespace YS.Knife.Data.Mappers
{
    public class FromQueryableAssignMapperExpression<TSource, TSourceValue, TTargetValue> : IMapperExpression
       where TSourceValue : TTargetValue
    {
        private readonly Type resultType;
        private readonly Expression<Func<TSource, IQueryable<TSourceValue>>> sourceExpression;
        public Type SourceValueType => typeof(TSourceValue);

        public FromQueryableAssignMapperExpression(Expression<Func<TSource, IQueryable<TSourceValue>>> sourceExpression, Type resultType)
        {
            this.sourceExpression = sourceExpression;
            this.resultType = resultType;
        }
        public LambdaExpression GetLambdaExpression()
        {
            var selectMethod = MethodFinder.GetQuerybleSelect<TSourceValue, TTargetValue>();
            var toResultMethod = resultType.IsArray ? MethodFinder.GetEnumerableToArray<TTargetValue>() : MethodFinder.GetEnumerableToList<TTargetValue>();
            var callSelectExpression = Expression.Call(selectMethod, this.sourceExpression.Body, GetAssignExpression());
            var toResultExpression = Expression.Call(toResultMethod, callSelectExpression);
            // 需要处理source为null的情况
            var resultExpression = Expression.Condition(
                 Expression.Equal(this.sourceExpression.Body, Expression.Constant(null))
                , Expression.Constant(null, resultType), toResultExpression);
            return Expression.Lambda(resultExpression, this.sourceExpression.Parameters.First());

        }

        private Expression<Func<TSourceValue, TTargetValue>> GetAssignExpression()
        {
            var p = Expression.Parameter(typeof(TSourceValue));
            return Expression.Lambda<Func<TSourceValue, TTargetValue>>(p, p);
        }
    }
}
