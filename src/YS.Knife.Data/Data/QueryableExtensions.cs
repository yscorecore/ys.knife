using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using YS.Knife.Data;

namespace System.Linq
{
    public static class LimitDataExtensions
    {
        public static IListSource AsListSource<T>(this ILimitData<T> limtData)
        {
            return new LimitDataListSource<T>(limtData);
        }
        public static LimitData<T> ToLimitData<T>(this IQueryable<T> source, int offset,int limit)
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
    }
}
