using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using YS.Knife.Data;
using YS.Knife.Data.Mappers;

namespace YS.Knife
{
    public static class MapperExtensions
    {
        static readonly Dictionary<string, MethodInfo> AllMethods = typeof(MapperExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => Attribute.IsDefined(m, typeof(DescriptionAttribute)))
            .ToDictionary(p => p.GetCustomAttribute<DescriptionAttribute>().Description);

        static readonly ConcurrentDictionary<string, MethodInfo> DelegateCache = new ConcurrentDictionary<string, MethodInfo>();

        static MethodInfo GetMethod(Type sourceType, Type targetType, string kind)
        {
            string key = $"{kind}_{sourceType.AssemblyQualifiedName}_{targetType.AssemblyQualifiedName}";
            return DelegateCache.GetOrAdd(key, (_) =>
                AllMethods[kind].MakeGenericMethod(sourceType, targetType)
            );
        }

        static T Invoke<T>(Type sourceType, Type targetType, string kind, params object[] arguments)
        {
            var methodInfo = GetMethod(sourceType, targetType, kind);
            return (T)methodInfo.Invoke(null, arguments);
        }

        public static TTarget Map<TSource, TTarget>(this TSource source, ObjectMapper<TSource, TTarget> mapper)
            where TSource : class
            where TTarget : class, new()
        {
            _ = mapper ?? throw new ArgumentNullException(nameof(mapper));
            return source == null ? null : mapper.BuildConvertFunc().Invoke(source);
        }

        [Description("map-single")]
        public static TTarget Map<TSource, TTarget>(this TSource source)
            where TSource : class
            where TTarget : class, new()
        {
            return source.Map(ObjectMapper<TSource, TTarget>.Default);
        }

        public static TTarget MapTo<TTarget>(this object source)
            where TTarget : class, new()
        {
            return source == null ? null : Invoke<TTarget>(source.GetType(), typeof(TTarget), "map-single", source);
        }

        public static IEnumerable<TTarget> Map<TSource, TTarget>(this IEnumerable<TSource> sources,
            ObjectMapper<TSource, TTarget> mapper)
            where TSource : class
            where TTarget : class, new()
        {
            _ = mapper ?? throw new ArgumentNullException(nameof(mapper));
            return sources?.Select(mapper.BuildConvertFunc());
        }

        [Description("map-enumerable")]
        public static IEnumerable<TTarget> Map<TSource, TTarget>(this IEnumerable<TSource> sources)
            where TSource : class
            where TTarget : class, new()
        {
            return sources.Map(ObjectMapper<TSource, TTarget>.Default);
        }

        public static IEnumerable<TTarget> MapTo<TTarget>(this IEnumerable sources)
            where TTarget : class, new()
        {
            if (sources == null) return null;
            var sourceItemType = sources.GetType().GetEnumerableItemType();
            Should.NotNull(sourceItemType, () => new InvalidOperationException($"can not judge source item type, source should be generic type '{typeof(IEnumerable<>)}'."));
            return Invoke<IEnumerable<TTarget>>(sourceItemType, typeof(TTarget), "map-enumerable", sources);
        }

        public static IQueryable<TTarget> Map<TSource, TTarget>(this IQueryable<TSource> source,
            ObjectMapper<TSource, TTarget> mapper)
            where TSource : class
            where TTarget : class, new()
        {
            _ = mapper ?? throw new ArgumentNullException(nameof(mapper));
            return source?.Select(mapper.BuildExpression());
        }

        [Description("map-queryable")]
        public static IQueryable<TTarget> Map<TSource, TTarget>(this IQueryable<TSource> sources)
            where TSource : class
            where TTarget : class, new()
        {
            return sources.Map(ObjectMapper<TSource, TTarget>.Default);
        }

        public static IQueryable<TTarget> MapTo<TTarget>(this IQueryable sources)
            where TTarget : class, new()
        {
            if (sources == null) return null;
            var sourceItemType = sources.GetType().GetQueryableItemType();
            Should.NotNull(sourceItemType, () => new InvalidOperationException($"can not judge source item type, , source should be generic type '{typeof(IQueryable<>)}'."));
            return Invoke<IQueryable<TTarget>>(sourceItemType, typeof(TTarget), "map-queryable", sources);
        }
    }
}
