using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using YS.Knife.Data.Mappers;
using YS.Knife.Data.Query;
using YS.Knife.Data.Query;

namespace System.Linq
{
    public static class QueryableExtensions
    {

        #region ListAll
        public static List<T> ListAll<T>(this IQueryable<T> source, QueryInfo queryInfo)
           where T : class, new()
        {
            return source.ListAll(
                FilterInfo.Parse(queryInfo?.Filter),
                OrderInfo.Parse(queryInfo?.OrderBy),
                SelectInfo.Parse(queryInfo?.Select));
        }
        public static List<T> ListAll<T>(this IQueryable<T> source, FilterInfo filter, OrderInfo orderBy, SelectInfo select)
            where T : class, new()
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            return source.DoFilter(filter)
                 .DoOrderBy(orderBy)
                 .DoSelect(select)
                 .ToList();
        }


        public static List<TTarget> ListAll<TSource, TTarget>(this IQueryable<TSource> source, QueryInfo queryInfo, ObjectMapper<TSource, TTarget> mapper)
            where TSource : class
            where TTarget : class, new()
        {

            return source.ListAll(
                FilterInfo.Parse(queryInfo?.Filter),
                OrderInfo.Parse(queryInfo?.OrderBy),
                SelectInfo.Parse(queryInfo?.Select),
                mapper);
        }
        public static List<TTarget> ListAll<TSource, TTarget>(this IQueryable<TSource> source, FilterInfo targetFilter, OrderInfo targetOrderBy, SelectInfo targetSelect, ObjectMapper<TSource, TTarget> mapper)
            where TSource : class
            where TTarget : class, new()
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = mapper ?? throw new ArgumentNullException(nameof(mapper));
            return source.DoFilter(targetFilter, mapper)
                .DoOrderBy(targetOrderBy, mapper)
                .DoSelect(targetSelect, mapper)
                .ToList();
        }
        #endregion


        #region ListLimit
        public static LimitList<T> ListLimit<T>(this IQueryable<T> source, LimitQueryInfo queryInfo)
           where T : class, new()
        {
            _ = queryInfo ?? throw new ArgumentNullException(nameof(queryInfo));
            return source.ListLimit(
                FilterInfo.Parse(queryInfo?.Filter),
                OrderInfo.Parse(queryInfo?.OrderBy),
                SelectInfo.Parse(queryInfo?.Select),
                queryInfo);
        }
        public static LimitList<T> ListLimit<T>(this IQueryable<T> source, FilterInfo filter, OrderInfo orderBy, SelectInfo select, ILimitInfo limitInfo)
            where T : class, new()
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = limitInfo ?? throw new ArgumentNullException(nameof(limitInfo));
            return source.DoFilter(filter)
                 .DoOrderBy(orderBy)
                 .DoSelect(select)
                 .ToLimitList(limitInfo);
        }


        public static LimitList<TTarget> ListLimit<TSource, TTarget>(this IQueryable<TSource> source, LimitQueryInfo queryInfo, ObjectMapper<TSource, TTarget> mapper)
            where TSource : class
            where TTarget : class, new()
        {

            return source.ListLimit(
                FilterInfo.Parse(queryInfo?.Filter),
                OrderInfo.Parse(queryInfo?.OrderBy),
                SelectInfo.Parse(queryInfo?.Select),
                queryInfo,
                mapper);
        }
        public static LimitList<TTarget> ListLimit<TSource, TTarget>(this IQueryable<TSource> source, FilterInfo targetFilter, OrderInfo targetOrderBy, SelectInfo targetSelect, ILimitInfo limitInfo, ObjectMapper<TSource, TTarget> mapper)
            where TSource : class
            where TTarget : class, new()
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = limitInfo ?? throw new ArgumentNullException(nameof(limitInfo));
            _ = mapper ?? throw new ArgumentNullException(nameof(mapper));
            return source.DoFilter(targetFilter, mapper)
                .DoOrderBy(targetOrderBy, mapper)
                .DoSelect(targetSelect, mapper)
                .ToLimitList(limitInfo);
        }
        #endregion


        #region ListPage
        public static PagedList<T> ListPage<T>(this IQueryable<T> source, LimitQueryInfo queryInfo)
           where T : class, new()
        {
            _ = queryInfo ?? throw new ArgumentNullException(nameof(queryInfo));
            return source.ListPage(
                FilterInfo.Parse(queryInfo?.Filter),
                OrderInfo.Parse(queryInfo?.OrderBy),
                SelectInfo.Parse(queryInfo?.Select),
                queryInfo);
        }
        public static PagedList<T> ListPage<T>(this IQueryable<T> source, FilterInfo filter, OrderInfo orderBy, SelectInfo select, ILimitInfo limitInfo)
            where T : class, new()
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = limitInfo ?? throw new ArgumentNullException(nameof(limitInfo));
            return source.DoFilter(filter)
                 .DoOrderBy(orderBy)
                 .DoSelect(select)
                 .ToPagedList(limitInfo);
        }


        public static PagedList<TTarget> ListPage<TSource, TTarget>(this IQueryable<TSource> source, LimitQueryInfo queryInfo, ObjectMapper<TSource, TTarget> mapper)
            where TSource : class
            where TTarget : class, new()
        {

            return source.ListPage(
                FilterInfo.Parse(queryInfo?.Filter),
                OrderInfo.Parse(queryInfo?.OrderBy),
                SelectInfo.Parse(queryInfo?.Select),
                queryInfo,
                mapper);
        }
        public static PagedList<TTarget> ListPage<TSource, TTarget>(this IQueryable<TSource> source, FilterInfo targetFilter, OrderInfo targetOrderBy, SelectInfo targetSelect, ILimitInfo limitInfo, ObjectMapper<TSource, TTarget> mapper)
            where TSource : class
            where TTarget : class, new()
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = limitInfo ?? throw new ArgumentNullException(nameof(limitInfo));
            _ = mapper ?? throw new ArgumentNullException(nameof(mapper));
            return source.DoFilter(targetFilter, mapper)
                .DoOrderBy(targetOrderBy, mapper)
                .DoSelect(targetSelect, mapper)
                .ToPagedList(limitInfo);
        }
        #endregion


        #region FirstOrDefault
        public static T FirstOrDefault<T>(this IQueryable<T> source, QueryInfo queryInfo)
           where T : class, new()
        {
            return source.FirstOrDefault(
                FilterInfo.Parse(queryInfo?.Filter),
                OrderInfo.Parse(queryInfo?.OrderBy),
                SelectInfo.Parse(queryInfo?.Select));
        }
        public static T FirstOrDefault<T>(this IQueryable<T> source, FilterInfo filter, OrderInfo orderBy, SelectInfo select)
            where T : class, new()
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            return source.DoFilter(filter)
                 .DoOrderBy(orderBy)
                 .DoSelect(select)
                 .FirstOrDefault();
        }


        public static TTarget FirstOrDefault<TSource, TTarget>(this IQueryable<TSource> source, QueryInfo queryInfo, ObjectMapper<TSource, TTarget> mapper)
            where TSource : class
            where TTarget : class, new()
        {

            return source.FirstOrDefault(
                FilterInfo.Parse(queryInfo?.Filter),
                OrderInfo.Parse(queryInfo?.OrderBy),
                SelectInfo.Parse(queryInfo?.Select),
                mapper);
        }
        public static TTarget FirstOrDefault<TSource, TTarget>(this IQueryable<TSource> source, FilterInfo targetFilter, OrderInfo targetOrderBy, SelectInfo targetSelect, ObjectMapper<TSource, TTarget> mapper)
            where TSource : class
            where TTarget : class, new()
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = mapper ?? throw new ArgumentNullException(nameof(mapper));
            return source.DoFilter(targetFilter, mapper)
                .DoOrderBy(targetOrderBy, mapper)
                .DoSelect(targetSelect, mapper)
                .FirstOrDefault();
        }
        #endregion
    }
}
