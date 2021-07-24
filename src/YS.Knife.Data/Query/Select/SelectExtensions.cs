using System;
using System.Linq;
using YS.Knife;
using YS.Knife.Data;
using YS.Knife.Data.Mappers;
using YS.Knife.Data.Query;

namespace System.Linq
{
    public static class SelectExtensions
    {
        public static IQueryable<T> DoSelect<T>(this IQueryable<T> source, SelectInfo selectInfo)
            where T : class, new()
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            return source.DoSelect<T, T>(selectInfo);
        }

        public static IQueryable<R> DoSelect<T, R>(this IQueryable<T> source, SelectInfo selectInfo, ObjectMapper<T, R> baseMapper)
            where T : class
            where R : class, new()
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            var mapper = baseMapper;

            return source.Map(mapper);
        }

        public static IQueryable<R> DoSelect<T, R>(this IQueryable<T> source, SelectInfo selectInfo)
            where T : class
            where R : class, new()
        {
            return source.DoSelect(selectInfo, ObjectMapper<T, R>.Default);
        }
    }
}
