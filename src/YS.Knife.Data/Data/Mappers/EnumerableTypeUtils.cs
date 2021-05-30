using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace YS.Knife.Data.Mappers
{
    static class EnumerableTypeUtils
    {
        private static readonly ConcurrentDictionary<Type,Type> EnumerableLocalCache = new ConcurrentDictionary<Type, Type>();

        private static readonly ConcurrentDictionary<Type, Type> QueryableLocalCache = new ConcurrentDictionary<Type, Type>();
        public static Type GetEnumerableItemType(Type enumableType)
        {
            if (enumableType.IsGenericTypeDefinition) return null;
            return EnumerableLocalCache.GetOrAdd(enumableType, type =>
                type.GetInterfaces()
                    .Where(p => p.IsGenericType && p.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    .Select(p => p.GetGenericArguments().First()).FirstOrDefault()
            ) ;
        }
        public static Type GetQueryableItemType(Type enumableType)
        {
            if (enumableType.IsGenericTypeDefinition) return null;
            return QueryableLocalCache.GetOrAdd(enumableType, type =>
                type.GetInterfaces()
                    .Where(p => p.IsGenericType && p.GetGenericTypeDefinition() == typeof(IQueryable<>))
                    .Select(p => p.GetGenericArguments().First()).FirstOrDefault()
            );
        }

    }
}
