using System.Collections.Generic;
using System.Linq;
using YS.Knife.Data.Mappers;

namespace YS.Knife
{
    public static class MapperExtensions
    {
        public static TTarget Map<TSource, TTarget>(this TSource source, ObjectMapper<TSource, TTarget> mapper)
            where TSource : class
            where TTarget : class, new()
        {
            if (source == null) return null;
            return mapper.BuildConvertFunc().Invoke(source);
        }
        public static TTarget Map<TSource, TTarget>(this TSource source)
            where TSource : class
            where TTarget : class, new()
        {
            return source.Map(ObjectMapper<TSource, TTarget>.Default);
        }
        public static IEnumerable<TTarget> Map<TSource, TTarget>(this IEnumerable<TSource> sources, ObjectMapper<TSource, TTarget> mapper)
           where TSource : class
           where TTarget : class, new()
        {
            if (sources == null) return null;
            var func = mapper.BuildConvertFunc();
            return sources.Select(func);
        }
        public static IEnumerable<TTarget> Map<TSource, TTarget>(this IEnumerable<TSource> sources)
         where TSource : class
         where TTarget : class, new()
        {
            return sources.Map(ObjectMapper<TSource, TTarget>.Default);
        }
        public static IQueryable<TTarget> Map<TSource, TTarget>(this IQueryable<TSource> source, ObjectMapper<TSource, TTarget> mapper)
            where TSource : class
            where TTarget : class, new()
        {
        
            return source?.Select(mapper.BuildExpression());
        }
        public static IQueryable<TTarget> Map<TSource, TTarget>(this IQueryable<TSource> sources)
           where TSource : class
           where TTarget : class, new()
        {

            return sources.Map(ObjectMapper<TSource, TTarget>.Default);
        }

        public static List<TTarget> MapList<TSource, TTarget>(this IEnumerable<TSource> sources, ObjectMapper<TSource, TTarget> mapper)
            where TSource : class
            where TTarget : class, new()
        {
            return sources?.Map(mapper)?.ToList();
        }
        public static List<TTarget> MapList<TSource, TTarget>(this IEnumerable<TSource> sources)
           where TSource : class
           where TTarget : class, new()
        {
            return sources.MapList(ObjectMapper<TSource, TTarget>.Default);
        }
        public static List<TTarget> MapList<TSource, TTarget>(this IQueryable<TSource> source, ObjectMapper<TSource, TTarget> mapper)
            where TSource : class
            where TTarget : class, new()
        {
            return source?.Map(mapper)?.ToList();
        }
        public static List<TTarget> MapList<TSource, TTarget>(this IQueryable<TSource> sources)
          where TSource : class
          where TTarget : class, new()
        {
            return sources.MapList(ObjectMapper<TSource, TTarget>.Default);
        }

        public static TTarget[] MapArray<TSource, TTarget>(this IEnumerable<TSource> source, ObjectMapper<TSource, TTarget> mapper)
              where TSource : class
            where TTarget : class, new()
        {
            return source?.Map(mapper)?.ToArray();
        }
        public static TTarget[] MapArray<TSource, TTarget>(this IEnumerable<TSource> sources)
           where TSource : class
         where TTarget : class, new()
        {
            return sources.MapArray(ObjectMapper<TSource, TTarget>.Default);
        }
        public static TTarget[] MapArray<TSource, TTarget>(this IQueryable<TSource> sources, ObjectMapper<TSource, TTarget> mapper)
              where TSource : class
            where TTarget : class, new()
        {
            return sources?.Map(mapper)?.ToArray();
        }
        public static TTarget[] MapArray<TSource, TTarget>(this IQueryable<TSource> sources)
             where TSource : class
           where TTarget : class, new()
        {
            return sources.MapArray(ObjectMapper<TSource, TTarget>.Default);
        }
    }
}
