using System;
using System.Collections.Generic;
using System.Linq;

namespace YS.Knife
{
    public static class TypeExtensions
    {
        private static readonly LocalCache<Type, Type> EnumerableLocalCache = new LocalCache<Type, Type>();
        private static readonly LocalCache<Type, Type> QueryableLocalCache = new LocalCache<Type, Type>();


        public static bool IsNullableType(this Type type)
        {
            return type != null && Nullable.GetUnderlyingType(type) != null;
        }

        public static (bool IsNullbale, Type UnderlyingType) GetUnderlyingTypeTypeInfo(this Type type)
        {
            return type.IsNullableType() ? (true, Nullable.GetUnderlyingType(type)) : (false, type);
        }


        public static Type GetEnumerableItemType(this Type enumerableType)
        {
            if (enumerableType.IsGenericTypeDefinition) return null;
            return EnumerableLocalCache.Get(enumerableType, type =>
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return type.GetGenericArguments().First();
                }
                return type.GetInterfaces()
                      .Where(p => p.IsGenericType && p.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                      .Select(p => p.GetGenericArguments().First()).FirstOrDefault();
            }
            );
        }
        public static Type GetQueryableItemType(this Type enumerableType)
        {
            if (enumerableType.IsGenericTypeDefinition) return null;
            return QueryableLocalCache.Get(enumerableType, type =>
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IQueryable<>))
                {
                    return type.GetGenericArguments().First();
                }
                return type.GetInterfaces()
                    .Where(p => p.IsGenericType && p.GetGenericTypeDefinition() == typeof(IQueryable<>))
                    .Select(p => p.GetGenericArguments().First()).FirstOrDefault();
            }
            );
        }

        public static bool IsEnumerable(this Type type) => GetEnumerableItemType(type) != null;
        public static bool IsQueryable(this Type type) => GetQueryableItemType(type) != null;
    }
}
