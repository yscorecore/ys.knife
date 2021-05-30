using System;
using System.Linq;
using System.Linq.Expressions;

namespace YS.Knife.Data.Mappers
{
    public class FromQueryableNewObjectMapperExpression<TSource, TSourceObjectValue, TTargetObjectValue> : IMapperExpression
       where TSourceObjectValue : class
       where TTargetObjectValue : class,new()
    {
        private readonly ObjectMapper<TSourceObjectValue, TTargetObjectValue> objectMapper;
        private readonly Expression<Func<TSource,IQueryable<TSourceObjectValue>>> sourceExpression;
        private readonly Type resultType;

        public Type SourceValueType => typeof(TSourceObjectValue);

        public FromQueryableNewObjectMapperExpression(Expression<Func<TSource, IQueryable<TSourceObjectValue>>> sourceExpression, ObjectMapper<TSourceObjectValue, TTargetObjectValue> objectMapper, Type resultType)
        {
            this.sourceExpression = sourceExpression;
            this.objectMapper = objectMapper;
            this.resultType = resultType;
        }



        public LambdaExpression GetLambdaExpression()
        {
            var newObjectExpression = this.objectMapper.BuildExpression();

            var selectMethod = MethodFinder.GetQuerybleSelect<TSourceObjectValue, TTargetObjectValue>() ;
            var toResultMethod = resultType.IsArray ? MethodFinder.GetEnumerableToArray<TTargetObjectValue>() : MethodFinder.GetEnumerableToList<TTargetObjectValue>();
            var callSelectExpression = Expression.Call(selectMethod, this.sourceExpression.Body, newObjectExpression);
            var toResultExpression = Expression.Call(toResultMethod, callSelectExpression);
            // 需要处理source为null的情况
            var resultExpression = Expression.Condition(
                 Expression.Equal(this.sourceExpression.Body, Expression.Constant(null))
                , Expression.Constant(null, resultType), toResultExpression);
            return Expression.Lambda(resultExpression, this.sourceExpression.Parameters.First());

        }

    }
}
