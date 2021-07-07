using System;
using System.Linq;
using YS.Knife;
using YS.Knife.Data;
using YS.Knife.Data.Mappers;

namespace System.Linq
{
    public static class SelectExtensions
    {
        public static IQueryable<T> Select<T>(this IQueryable<T> source, SelectInfo selectInfo)
            where T:class,new()
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            return source.Select<T, T>(selectInfo);
        }
        
        public static IQueryable<R> Select<T,R>(this IQueryable<T> source, SelectInfo selectInfo, ObjectMapper<T,R> baseMapper)
            where T:class
            where R:class,new()
        {
            _ = source ?? throw new ArgumentNullException(nameof(source));
            var mapper =selectInfo?.Include==null?baseMapper: baseMapper.Pick(selectInfo?.Include);

            return source.Map(mapper);
        }
        
        public static IQueryable<R> Select<T,R>(this IQueryable<T> source, SelectInfo selectInfo)
            where T:class
            where R:class,new()
        {
            return source.Select(selectInfo, ObjectMapper<T, R>.Default);
        }
    }
}
