using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YS.Knife.Data;
using YS.Knife.Data.Query;

namespace System.Linq
{
    public static class LimitExtensions
    {
        #region Limit
        public static IListSource ToListSource<T>(this ILimitList<T> limitList)
        {
            return new LimitListSource<T>(limitList);
        }
        public static LimitList<T> ToLimitList<T>(this IQueryable<T> source, int offset, int limit)
        {
            return source.ToLimitList(new LimitInfo(offset, limit));
        }
        public static LimitList<T> ToLimitList<T>(this IQueryable<T> source, ILimitInfo limitInfo)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = limitInfo ?? throw new ArgumentNullException(nameof(limitInfo));
            var limit = limitInfo.Limit;
            var offset = limitInfo.Offset;
            var limitListData = source.Skip(offset).Take(limit).ToList();
            return new LimitList<T>(limitListData, offset, limit);
        }
        public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int offset, int limit)
        {
            return source.ToPagedList(new LimitInfo(offset, limit));
        }
        public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, ILimitInfo limitInfo)
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            _ = limitInfo ?? throw new ArgumentNullException(nameof(limitInfo));
            var limit = limitInfo.Limit;
            var offset = limitInfo.Offset;
            var totalCount = source.Count();
            var limitListData = source.Skip(offset).Take(limit).ToList();
            return new PagedList<T>(limitListData, offset, limit, totalCount);
        }
        #endregion
    }
}
