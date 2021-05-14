using System.Collections.Generic;
using System.Linq;

namespace YS.Knife.Data.Mapper
{
    public static class ObjectExtensions
    {
        public static TTo MapOne<TFrom, TTo>(this TFrom source, ObjectMapper<TFrom, TTo> mapper)
        where TTo: new()
        {
            return mapper.GetFunc().Invoke(source);
        }
        public static IQueryable<TTo> MapQueryable<TFrom, TTo>(this IQueryable<TFrom> source, ObjectMapper<TFrom, TTo> mapper)
            where TTo: new()
        {
            return source.Select(mapper.GetExpression());
        }
        public static List<TTo> MapList<TFrom, TTo>(this IEnumerable<TFrom> source, ObjectMapper<TFrom, TTo> mapper)
            where TTo: new()
        {
            var func = mapper.GetFunc();
            return (source ?? Enumerable.Empty<TFrom>()).Select(item => func(item)).ToList();
        }
        public static TTo[] MapArray<TFrom, TTo>(this IEnumerable<TFrom> source, ObjectMapper<TFrom, TTo> mapper)
            where TTo: new()
        {
            var func = mapper.GetFunc();
            return (source ?? Enumerable.Empty<TFrom>()).Select(item => func(item)).ToArray();
        }
    }
}
