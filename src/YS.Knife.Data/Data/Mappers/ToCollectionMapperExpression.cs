using System;
using System.Linq;
using System.Linq.Expressions;

namespace YS.Knife.Data.Mappers
{
    public class ToCollectionMapperExpression<TSource, TTarget> : IMapperExpression
        where TSource : class
        where TTarget : class
    {
        private readonly ObjectMapper<TSource, TTarget> objectMapper;
        private readonly LambdaExpression sourceExpression;
        private readonly Type resultType;

        public Type SourceValueType => throw new NotImplementedException();

        public ToCollectionMapperExpression(LambdaExpression sourceExpression, ObjectMapper<TSource, TTarget> objectMapper, Type resultType)
        {
            this.sourceExpression = sourceExpression;
            this.objectMapper = objectMapper;
            this.resultType = resultType;
        }

        private bool IsQueryableSource()
        {
            return typeof(IQueryable<TSource>).IsAssignableFrom(this.sourceExpression.ReturnType);
          
        }
            
        public LambdaExpression GetLambdaExpression()
        {
            var newObjectExpression = this.objectMapper.BuildExpression();

            var selectMethod = IsQueryableSource()? MethodFinder.GetQuerybleSelect<TSource, TTarget>(): MethodFinder.GetEnumerableSelect<TSource, TTarget>();
            var toResultMethod = resultType.IsArray ? MethodFinder.GetEnumerableToArray<TTarget>() : MethodFinder.GetEnumerableToList<TTarget>();
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
