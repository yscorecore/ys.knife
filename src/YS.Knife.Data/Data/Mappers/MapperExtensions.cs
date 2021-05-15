using System.Collections.Generic;
using System.Linq;
using YS.Knife.Data.Mappers;

namespace YS.Knife
{
    public static class MapperExtensions
    {
        public static TTarget Map<TSoruce, TTarget>(this TSoruce source, ObjectMapper<TSoruce, TTarget> mapper)
            where TSoruce : class
            where TTarget : class, new()
        {
            if (source == null) return null;
            return mapper.BuildConvertFunc().Invoke(source);
        }
        public static TTarget Map<TSoruce, TTarget>(this TSoruce source)
            where TSoruce : class
            where TTarget : class, new()
        {
            return source.Map(ObjectMapper<TSoruce, TTarget>.Default);
        }
        public static IEnumerable<TTarget> Map<TSoruce, TTarget>(this IEnumerable<TSoruce> sources, ObjectMapper<TSoruce, TTarget> mapper)
           where TSoruce : class
           where TTarget : class, new()
        {
            if (sources == null) return null;
            var func = mapper.BuildConvertFunc();
            return sources.Select(func);
        }
        public static IEnumerable<TTarget> Map<TSoruce, TTarget>(this IEnumerable<TSoruce> sources)
         where TSoruce : class
         where TTarget : class, new()
        {
            return sources.Map(ObjectMapper<TSoruce, TTarget>.Default);
        }
        public static IQueryable<TTarget> Map<TSoruce, TTarget>(this IQueryable<TSoruce> source, ObjectMapper<TSoruce, TTarget> mapper)
            where TSoruce : class
            where TTarget : class, new()
        {

            if (source == null) return null;
            return source.Select(mapper.BuildExpression());
        }
        public static IQueryable<TTarget> Map<TSoruce, TTarget>(this IQueryable<TSoruce> sources)
           where TSoruce : class
           where TTarget : class, new()
        {

            return sources.Map(ObjectMapper<TSoruce, TTarget>.Default);
        }

        public static List<TTarget> MapList<TSoruce, TTarget>(this IEnumerable<TSoruce> sources, ObjectMapper<TSoruce, TTarget> mapper)
            where TSoruce : class
            where TTarget : class, new()
        {
            if (sources == null) return null;
            return sources.Map(mapper).ToList();
        }
        public static List<TTarget> MapList<TSoruce, TTarget>(this IEnumerable<TSoruce> sources)
           where TSoruce : class
           where TTarget : class, new()
        {
            return sources.MapList(ObjectMapper<TSoruce, TTarget>.Default);
        }
        public static List<TTarget> MapList<TSoruce, TTarget>(this IQueryable<TSoruce> source, ObjectMapper<TSoruce, TTarget> mapper)
            where TSoruce : class
            where TTarget : class, new()
        {
            if (source == null) return null;
            return source.Map(mapper).ToList();
        }
        public static List<TTarget> MapList<TSoruce, TTarget>(this IQueryable<TSoruce> sources)
          where TSoruce : class
          where TTarget : class, new()
        {
            return sources.MapList(ObjectMapper<TSoruce, TTarget>.Default);
        }

        public static TTarget[] MapArray<TSoruce, TTarget>(this IEnumerable<TSoruce> source, ObjectMapper<TSoruce, TTarget> mapper)
              where TSoruce : class
            where TTarget : class, new()
        {
            if (source == null) return null;
            return source.Map(mapper).ToArray();
        }
        public static TTarget[] MapArray<TSoruce, TTarget>(this IEnumerable<TSoruce> sources)
           where TSoruce : class
         where TTarget : class, new()
        {
            return sources.MapArray(ObjectMapper<TSoruce, TTarget>.Default);
        }
        public static TTarget[] MapArray<TSoruce, TTarget>(this IQueryable<TSoruce> sources, ObjectMapper<TSoruce, TTarget> mapper)
              where TSoruce : class
            where TTarget : class, new()
        {
            if (sources == null) return null;
            return sources.Map(mapper).ToArray();
        }
        public static TTarget[] MapArray<TSoruce, TTarget>(this IQueryable<TSoruce> sources)
             where TSoruce : class
           where TTarget : class, new()
        {
            return sources.MapArray(ObjectMapper<TSoruce, TTarget>.Default);
        }
    }
}
