using System.Collections.Generic;
using System.ComponentModel;
using YS.Knife.Data;

namespace System.Linq
{
    public static class QueryableExtensions
    {
        public static IListSource AsListSource<T>(this ILimitData<T> limitData)
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

        #region List
        public static List<T> ListAll<T>(this IQueryable<T> source, FilterInfo filterInfo, OrderInfo orderInfo)
        {
            return source.WhereCondition(filterInfo).Order(orderInfo).ToList();
        }
        public static List<T> ListAll<T>(this IQueryable<T> source, QueryInfo queryInfo)
        {
            // ignore limit info
            return source.ListAll<T>(queryInfo?.Filter,queryInfo?.Order);
        }
        public static List<R> ListAll<T,R>(this IQueryable<T> source, FilterInfo filterInfoForTFrom, OrderInfo orderInfoForTFrom, Func<IQueryable<T>,IQueryable<R>> mapper=null)
        {
            return null;
        }
        public static List<R> ListAll<T,R>(this IQueryable<T> source, QueryInfo queryInfoForTFrom, Func<IQueryable<T>,IQueryable<R>> mapper=null)
        {
            // ignore limit info
            return source.ListAll(queryInfoForTFrom?.Filter,queryInfoForTFrom?.Order,mapper);
        }
        

        #endregion

        
    }
}
