using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using YS.Knife.Data;
using YS.Knife.Data.Filter;
using YS.Knife.Data.Mappers;
using YS.Knife.Data.Query;

namespace System.Linq
{
    public static class QueryableExtensions
    {
       

        #region Where
        public static IQueryable<TSource> Where<TSource, TTarget>(this IQueryable<TSource> source, FilterInfo targetFilter, ObjectMapper<TSource, TTarget> mapper)
            where TSource : class
            where TTarget : class, new()
        {
            return targetFilter == null ? source : source.Where(FilterInfoExpressionBuilder.Default.CreateFilterLambdaExpression(mapper, targetFilter));
        }
        public static IQueryable<T> Where<T>(this IQueryable<T> source, FilterInfo filter)
        {
            return filter == null ? source : source.Where(FilterInfoExpressionBuilder.Default.CreateFilterLambdaExpression<T>(filter));
        }
        #endregion

        #region ListAll
        public static List<T> ListAll<T>(this IQueryable<T> source, QueryInfo queryInfo)
           where T : class, new()
        {
            return null;
            // ignore limit info
            // return source.ListAll(queryInfo?.Filter, queryInfo?.Order, queryInfo?.Select);
        }
        public static List<T> ListAll<T>(this IQueryable<T> source, FilterInfo filter, OrderInfo order, SelectInfo select)
            where T : class, new()
        {
            return source.Where(filter)
                 .Select(select)
                 .ToList();
        }
        public static List<TTarget> ListAll<TSource, TTarget>(this IQueryable<TSource> source, QueryInfo queryInfo, ObjectMapper<TSource, TTarget> mapper)
            where TSource : class
            where TTarget : class, new()
        {
            return null;
            // ignore limit info
            //return source.ListAll(queryInfo?.Filter, queryInfo?.Order, queryInfo?.Select, mapper);
        }
        public static List<TTarget> ListAll<TSource, TTarget>(this IQueryable<TSource> source, FilterInfo targetFilter, OrderInfo targetOrder, SelectInfo targetSelect, ObjectMapper<TSource, TTarget> mapper)
            where TSource : class
            where TTarget : class, new()
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = mapper ?? throw new ArgumentNullException(nameof(mapper));



            return source.Where(targetFilter, mapper)
            //    .Order(null)
                .Select(targetSelect, mapper)
            .ToList();
        }
        public static List<R> ListAll<T, R>(this IQueryable<T> source, FilterInfo targetFilter, OrderInfo targetOrder, SelectInfo selectInfoForResult)
            where T : class
            where R : class, new()
        {
            return source.ListAll(targetFilter, targetOrder, selectInfoForResult, ObjectMapper<T, R>.Default);
        }
        #endregion


    }
}
