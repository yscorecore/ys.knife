using System;
using System.Collections.Generic;
using System.Linq;

namespace YS.Knife.Data
{
    public static class TypeExtensions
    {
        private static readonly LocalCache<Type, Type> EnumerableLocalCache = new LocalCache<Type, Type>();

        public static bool IsGenericEnumerable(this Type enumableType)
        {
            return EnumerableLocalCache.Get(enumableType, type =>
                type.GetInterfaces()
                    .Where(p => p.IsGenericType && p.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    .Select(p => p.GetGenericArguments().First()).FirstOrDefault()
            ) != null;
        }

        public static Type GetEnumerableSubType(this Type enumableType)
        {
            if (IsGenericEnumerable(enumableType))
            {
                return EnumerableLocalCache.Get(enumableType);
            }
            throw new InvalidOperationException(
                $"Can not get sub item type from type '{enumableType.FullName}', because it's not extend type '{typeof(IEnumerable<>).FullName}'.");
        }

        public static bool IsNullableType(this Type type)
        {
            return type != null && Nullable.GetUnderlyingType(type) != null;
        }

        public static (bool IsNullbale, Type UnderlyingType) GetUnderlyingTypeTypeInfo(this Type type)
        {
            return type.IsNullableType() ? (true, Nullable.GetUnderlyingType(type)) : (false, type);
        }
    }
}
