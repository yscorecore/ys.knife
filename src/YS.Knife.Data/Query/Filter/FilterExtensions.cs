using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YS.Knife.Data;
using YS.Knife.Data.Filter;
using YS.Knife.Data.Mappers;

namespace System.Linq
{
    public static class FilterExtensions
    {
        public static IQueryable<TSource> DoFilter<TSource, TTarget>(this IQueryable<TSource> source, FilterInfo targetFilter, ObjectMapper<TSource, TTarget> mapper)
            where TSource : class
            where TTarget : class, new()
        {
            return targetFilter == null ? source : source.Where(FilterInfoExpressionBuilder.Default.CreateFilterLambdaExpression(mapper, targetFilter));
        }
        public static IQueryable<T> DoFilter<T>(this IQueryable<T> source, FilterInfo filter)
        {
            return filter == null ? source : source.Where(FilterInfoExpressionBuilder.Default.CreateFilterLambdaExpression<T>(filter));
        }
    }
}
