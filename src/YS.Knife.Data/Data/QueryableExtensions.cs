using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using YS.Knife.Data;
using YS.Knife.Data.Mappers;

namespace System.Linq
{
    public static class QueryableExtensions
    {
        #region Other
        public static IListSource ToListSource<T>(this ILimitData<T> limitData)
        {
            return new LimitDataListSource<T>(limitData);
        }
        public static LimitData<T> ToLimitData<T>(this IQueryable<T> source, int offset, int limit)
        {
            return source.ToLimitData(new LimitInfo(offset, limit));
        }
        public static LimitData<T> ToLimitData<T>(this IQueryable<T> source, LimitInfo limitInfo)
        {
            _ = limitInfo ?? throw new ArgumentNullException(nameof(limitInfo));
            var limit = limitInfo.Limit;
            var offset = limitInfo.Offset;
            var totalCount = source.Count();
            var limitListData = source.Skip(offset).Take(limit).ToList();
            return new LimitData<T>(limitListData, offset, limit, totalCount);
        }

        

        #endregion

        #region ListAll
        public static List<T> ListAll<T>(this IQueryable<T> source, FilterInfo filterInfoForResult, OrderInfo orderInfoForResult, SelectInfo selectInfoForResult)
            where T:class,new()
        {
            return source.ListAll<T,T>(filterInfoForResult, orderInfoForResult, selectInfoForResult);
        }
        public static List<T> ListAll<T>(this IQueryable<T> source, QueryInfo queryInfo)
            where T:class,new()
        {
            // ignore limit info
            return source.ListAll(queryInfo?.Filter,queryInfo?.Order,queryInfo?.Select);
        }
        public static List<R> ListAll<T,R>(this IQueryable<T> source, FilterInfo filterInfoForResult, OrderInfo orderInfoForResult, SelectInfo selectInfoForResult, ObjectMapper<T,R> mapper)
            where  T:class
            where  R:class, new()
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = mapper ?? throw new ArgumentNullException(nameof(mapper));

            return null;

            //return source
            //    .Where(null)
            //    .Order(null)
            //    .Select(selectInfoForResult,mapper)
            //    .ToList();
        }
        public static List<R> ListAll<T,R>(this IQueryable<T> source, FilterInfo filterInfoForResult, OrderInfo orderInfoForResult, SelectInfo selectInfoForResult)
            where  T:class
            where  R:class, new()
        {
            return source.ListAll(filterInfoForResult, orderInfoForResult, selectInfoForResult, ObjectMapper<T, R>.Default);
        }
        #endregion

        
    }
}
