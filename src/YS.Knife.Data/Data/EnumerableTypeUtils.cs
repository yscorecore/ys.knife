using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace YS.Knife.Data
{
    static class EnumerableTypeUtils
    {
        private static readonly ConcurrentDictionary<Type, Type> EnumerableLocalCache = new ConcurrentDictionary<Type, Type>();

        private static readonly ConcurrentDictionary<Type, Type> QueryableLocalCache = new ConcurrentDictionary<Type, Type>();
        public static Type GetEnumerableItemType(Type enumerableType)
        {
            if (enumerableType.IsGenericTypeDefinition) return null;
            return EnumerableLocalCache.GetOrAdd(enumerableType, type =>
            {
                if (type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return type.GetGenericArguments().First();
                }
                return type.GetInterfaces()
                      .Where(p => p.IsGenericType && p.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                      .Select(p => p.GetGenericArguments().First()).FirstOrDefault();
            }
            );
        }
        public static Type GetQueryableItemType(Type enumerableType)
        {
            if (enumerableType.IsGenericTypeDefinition) return null;
            return QueryableLocalCache.GetOrAdd(enumerableType, type =>
            {
                if (type.GetGenericTypeDefinition() == typeof(IQueryable<>))
                {
                    return type.GetGenericArguments().First();
                }
                return type.GetInterfaces()
                    .Where(p => p.IsGenericType && p.GetGenericTypeDefinition() == typeof(IQueryable<>))
                    .Select(p => p.GetGenericArguments().First()).FirstOrDefault();
            }
            );
        }

        public static bool IsEnumerable(Type type) => GetEnumerableItemType(type) != null;
        public static bool IsQueryable(Type type) => GetQueryableItemType(type) != null;

    }
}
