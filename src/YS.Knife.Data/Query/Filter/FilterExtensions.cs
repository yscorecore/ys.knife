using System.Linq.Expressions;
using YS.Knife;
using YS.Knife.Data;
using YS.Knife.Data.Filter.Operators;
using YS.Knife.Data.Mappers;
using YS.Knife.Data.Query;
using YS.Knife.Data.Query.Expressions;

namespace System.Linq
{
    public static class FilterExtensions
    {

        public static IQueryable<T> DoFilter<T>(this IQueryable<T> source, FilterInfo filter)
        {
            return filter == null ? source : source.Where(CreateFilterLambdaExpression<T>(filter));
        }

        private static Expression<Func<T, bool>> CreateFilterLambdaExpression<T>(FilterInfo filterInfo)
        {
            var p = Expression.Parameter(typeof(T), "p");
            var expression = CreateFilterExpression<T>(p, filterInfo);
            return Expression.Lambda<Func<T, bool>>(expression, p);
        }


        private static Expression CreateFilterExpression<T>(ParameterExpression p, FilterInfo filterInfo)
        {
            if (filterInfo == null)
            {
                return Expression.Constant(true);
            }
            else
            {
                return CreateCombinGroupsFilterExpression<T>(p, filterInfo);
            }
        }
        private static Expression CreateCombinGroupsFilterExpression<T>(ParameterExpression p, FilterInfo filterInfo)
        {
            return filterInfo.OpType switch
            {
                CombinSymbol.AndItems => CreateAndConditionFilterExpression<T>(p, filterInfo),
                CombinSymbol.OrItems => CreateOrConditionFilterExpression<T>(p, filterInfo),
                _ => CreateSingleItemFilterExpression<T>(p, filterInfo)
            };
        }

        private static Expression CreateOrConditionFilterExpression<T>(ParameterExpression p, FilterInfo orGroupFilterInfo)
        {
            Expression current = null;
            foreach (FilterInfo item in orGroupFilterInfo.Items.TrimNotNull())
            {
                var next = CreateCombinGroupsFilterExpression<T>(p, item);
                current = current == null ? next : Expression.OrElse(current, next);
            }
            return current ?? Expression.Constant(true);
        }
        private static Expression CreateAndConditionFilterExpression<T>(ParameterExpression p, FilterInfo andGroupFilterInfo)
        {

            Expression current = null;
            foreach (FilterInfo item in andGroupFilterInfo.Items.TrimNotNull())
            {
                var next = CreateCombinGroupsFilterExpression<T>(p, item);
                current = current == null ? next : Expression.AndAlso(current, next);
            }
            return current ?? Expression.Constant(true);
        }

        private static Expression CreateSingleItemFilterExpression<TSource>(ParameterExpression p, FilterInfo singleItemFilter)
        {
            var leftExpressionValue = new ExpressionValue
            (
                typeof(TSource),
               singleItemFilter.Left,
                IMemberVisitor.GetObjectVisitor(typeof(TSource))
            );
            var rightExpressionValue = new ExpressionValue
            (
                typeof(TSource),
                singleItemFilter.Right,
                IMemberVisitor.GetObjectVisitor(typeof(TSource))
            );

            var lambda = IFilterOperator.CreateOperatorLambda(leftExpressionValue, singleItemFilter.Operator, rightExpressionValue);
            return lambda.ReplaceFirstParam(p);
        }

        public static IQueryable<TSource> DoFilter<TSource, TTarget>(this IQueryable<TSource> source, FilterInfo targetFilter, ObjectMapper<TSource, TTarget> mapper)
        where TSource : class
        where TTarget : class, new()
        {
            return targetFilter == null ? source : source.Where(CreateFilterLambdaExpression(targetFilter, mapper));
        }
        private static Expression<Func<TSource, bool>> CreateFilterLambdaExpression<TSource, TTarget>(
         FilterInfo targetFilter, ObjectMapper<TSource, TTarget> mapper)
        where TSource : class
        where TTarget : class, new()
        {
            _ = mapper ?? throw new ArgumentNullException(nameof(mapper));
            var p = Expression.Parameter(typeof(TSource), "p");
            var expression = CreateFilterExpression(p, targetFilter, mapper);
            return Expression.Lambda<Func<TSource, bool>>(expression, p);
        }

        private static Expression CreateFilterExpression<TSource, TTarget>(ParameterExpression p, FilterInfo filterInfo, ObjectMapper<TSource, TTarget> mapper)
                 where TSource : class
                where TTarget : class, new()
        {
            if (filterInfo == null)
            {
                return Expression.Constant(true);
            }
            else
            {
                return CreateCombinGroupsFilterExpression(p, filterInfo, mapper);
            }
        }

        private static Expression CreateCombinGroupsFilterExpression<TSource, TTarget>(ParameterExpression p, FilterInfo filterInfo, ObjectMapper<TSource, TTarget> mapper)
            where TSource : class
            where TTarget : class, new()
        {
            return filterInfo.OpType switch
            {
                CombinSymbol.AndItems => CreateAndConditionFilterExpression(p, filterInfo, mapper),
                CombinSymbol.OrItems => CreateOrConditionFilterExpression(p, filterInfo, mapper),
                _ => CreateSingleItemFilterExpression(p, filterInfo, mapper)
            };
        }


        private static Expression CreateOrConditionFilterExpression<TSource, TTarget>(ParameterExpression p, FilterInfo orGroupFilterInfo, ObjectMapper<TSource, TTarget> mapper)
            where TSource : class
            where TTarget : class, new()
        {
            Expression current = null;
            foreach (FilterInfo item in orGroupFilterInfo.Items.TrimNotNull())
            {
                var next = CreateCombinGroupsFilterExpression(p, item, mapper);
                current = current == null ? next : Expression.OrElse(current, next);
            }
            return current ?? Expression.Constant(true);
        }

        private static Expression CreateAndConditionFilterExpression<TSource, TTarget>(ParameterExpression p, FilterInfo andGroupFilterInfo, ObjectMapper<TSource, TTarget> mapper)
            where TSource : class
            where TTarget : class, new()
        {
            Expression current = null;
            foreach (FilterInfo item in andGroupFilterInfo.Items.TrimNotNull())
            {
                var next = CreateCombinGroupsFilterExpression(p, item, mapper);
                current = current == null ? next : Expression.AndAlso(current, next);
            }
            return current ?? Expression.Constant(true);
        }

        private static Expression CreateSingleItemFilterExpression<TSource, TTarget>(ParameterExpression p, FilterInfo singleItemFilter, ObjectMapper<TSource, TTarget> mapper)
          where TSource : class
          where TTarget : class, new()
        {
            var leftExpressionValue = new ExpressionValue
            (
                 typeof(TSource),
                singleItemFilter.Left,
                IMemberVisitor.GetMapperVisitor(mapper)
            );
            var rightExpressionValue = new ExpressionValue
            (
                typeof(TSource),
               singleItemFilter.Right,
               IMemberVisitor.GetMapperVisitor(mapper)
            );

            var lambda = IFilterOperator.CreateOperatorLambda(leftExpressionValue, singleItemFilter.Operator, rightExpressionValue);
            return lambda.ReplaceFirstParam(p);
        }

    }
}
