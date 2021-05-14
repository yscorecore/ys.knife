using System.Collections.Generic;
using System.Linq;

namespace YS.Knife.Data.Mapper
{
    public static class ObjectExtensions
    {
        public static TTarget MapOne<TSoruce, TTarget>(this TSoruce source, ObjectMapper<TSoruce, TTarget> mapper)
            where TSoruce:class, new()
            where TTarget:class, new()
        {
            if (source == null) return null;
            return mapper.GetFunc().Invoke(source);
        }
        public static IQueryable<TTarget> MapQueryable<TSoruce, TTarget>(this IQueryable<TSoruce> source, ObjectMapper<TSoruce, TTarget> mapper)
            where TSoruce : class, new()
            where TTarget : class, new()
        {
            return source.Select(mapper.GetExpression());
        }
        public static List<TTarget> MapList<TSoruce, TTarget>(this IEnumerable<TSoruce> source, ObjectMapper<TSoruce, TTarget> mapper)
            where TSoruce : class, new()
            where TTarget : class, new()
        {
            var func = mapper.GetFunc();
            return (source ?? Enumerable.Empty<TSoruce>()).Select(item => func(item)).ToList();
        }
        public static TTarget[] MapArray<TSoruce, TTarget>(this IEnumerable<TSoruce> source, ObjectMapper<TSoruce, TTarget> mapper)
              where TSoruce : class, new()
            where TTarget : class, new()
        {
            var func = mapper.GetFunc();
            return (source ?? Enumerable.Empty<TSoruce>()).Select(item => func(item)).ToArray();
        }
    }
}
