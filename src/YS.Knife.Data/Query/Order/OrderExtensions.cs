using System.Collections.Generic;
using System.Linq.Expressions;
using YS.Knife;
using YS.Knife.Data;
using YS.Knife.Data.Mappers;
using YS.Knife.Data.Query;
using YS.Knife.Data.Query.Expressions;

namespace System.Linq
{
    public static class OrderExtensions
    {
        public static IQueryable<T> DoOrderBy<T>(this IQueryable<T> source, OrderInfo orderInfo)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            return source.DoOrderBy(orderInfo?.Items ?? Enumerable.Empty<OrderItem>());
        }

        public static IQueryable<TSource> DoOrderBy<TSource, TTarget>(this IQueryable<TSource> source,
            OrderInfo orderInfo, ObjectMapper<TSource, TTarget> mapper)
            where TSource : class
            where TTarget : class, new()
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            return source.DoOrderBy(orderInfo?.Items ?? Enumerable.Empty<OrderItem>(), mapper);
        }
        public static IQueryable<TSource> DoOrderBy<TSource>(this IQueryable<TSource> source,
                params (LambdaExpression KeySelector, OrderType OrderType)[] orderByRules)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            IOrderedQueryable<TSource> result = null;
            foreach (var (lambda, type) in orderByRules)
            {
                if (lambda == null) continue;

                if (type == OrderType.Asc)
                {
                    //顺序
                    if (result != null)
                    {
                        result = result.ThenAsc(lambda);
                    }
                    else
                    {
                        result = source.Asc(lambda);
                    }
                }
                else
                {
                    if (result != null)
                    {
                        result = result.ThenDesc(lambda);
                    }
                    else
                    {
                        result = source.Desc(lambda);
                    }
                }
            }

            return result ?? source;
        }
        private static IQueryable<T> DoOrderBy<T>(this IQueryable<T> source, IEnumerable<OrderItem> orderItems)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            var lambdas = orderItems
                .TrimNotNull()
                .Select(item => (CreateKeySelectorLambda<T>(item.NavigatePaths), item.OrderType))
                .ToArray();

            return source.DoOrderBy(lambdas);
        }

        private static IQueryable<TSource> DoOrderBy<TSource, TTarget>(this IQueryable<TSource> source,
            IEnumerable<OrderItem> orderItems, ObjectMapper<TSource, TTarget> mapper)
            where TSource : class
            where TTarget : class, new()
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            var lambdas = orderItems
                .TrimNotNull()
                .Select(item => (CreateKeySelectorLambda(item.NavigatePaths, mapper), item.OrderType))
                .ToArray();

            return source.DoOrderBy(lambdas);
        }

        private static LambdaExpression CreateKeySelectorLambda<TSource>(List<ValuePath> valuePaths)
        {
            var expValue = new ExpressionValue(typeof(TSource), ValueInfo.FromPaths(valuePaths), IMemberVisitor.GetObjectVisitor(typeof(TSource)));
            return expValue.GetLambda();
        }

        private static LambdaExpression CreateKeySelectorLambda<TSource, TTarget>(List<ValuePath> valuePaths,
            ObjectMapper<TSource, TTarget> mapper)
            where TSource : class
            where TTarget : class, new()
        {
            var expValue = new ExpressionValue(typeof(TSource), ValueInfo.FromPaths(valuePaths), IMemberVisitor.GetMapperVisitor(mapper));
            return expValue.GetLambda();

        }



        private static IOrderedQueryable<T> Asc<T>(this IQueryable<T> source, LambdaExpression keySelector)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = keySelector ?? throw new ArgumentNullException(nameof(keySelector));

            return DoOrder(source, nameof(Queryable.OrderBy), keySelector);
        }

        private static IOrderedQueryable<T> ThenAsc<T>(this IQueryable<T> source, LambdaExpression keySelector)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = keySelector ?? throw new ArgumentNullException(nameof(keySelector));

            return DoOrder(source, nameof(Queryable.ThenBy), keySelector);
        }

        private static IOrderedQueryable<T> Desc<T>(this IQueryable<T> source, LambdaExpression keySelector)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
            return DoOrder(source, nameof(Queryable.OrderByDescending), keySelector);
        }

        private static IOrderedQueryable<T> ThenDesc<T>(this IQueryable<T> source, LambdaExpression keySelector)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
            return DoOrder(source, nameof(Queryable.ThenByDescending), keySelector);
        }

        private static IOrderedQueryable<TSource> DoOrder<TSource>(IQueryable<TSource> source, string methodName,
            LambdaExpression keySelector)
        {
            var types = new Type[] { typeof(TSource), keySelector.ReturnType };
            Expression expr = Expression.Call(typeof(Queryable),
                methodName, types, source.Expression, keySelector);
            return source.Provider.CreateQuery<TSource>(expr) as IOrderedQueryable<TSource>;
        }
    }
}
