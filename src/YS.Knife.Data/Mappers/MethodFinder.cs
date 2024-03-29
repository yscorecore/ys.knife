﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using static System.Math;

namespace YS.Knife.Data.Mappers
{
    static class MethodFinder
    {
        static readonly MethodInfo EnumerableSelectMethod = typeof(Enumerable)
            .GetMethods()
            .Where(p => p.Name == nameof(Enumerable.Select))
            .Where(p => p.GetParameters().Length == 2).Single(p => p.GetParameters().Last().ParameterType.GetGenericTypeDefinition() == typeof(Func<,>));

        static readonly MethodInfo QuerySelectMethod = typeof(Queryable).GetMethods()
            .Where(p => p.Name == nameof(Queryable.Select))
            .Where(p => p.GetParameters().Length == 2)
            .Single(p =>
                p.GetParameters().Last().ParameterType.GetGenericArguments().First().GetGenericTypeDefinition() ==
                typeof(Func<,>));

        private static readonly MethodInfo AsQueryableMethod = typeof(Queryable).GetMethods()
            .Single(p => p.Name == nameof(Queryable.AsQueryable) && p.IsGenericMethodDefinition);


        static readonly MethodInfo EnumerableToArrayMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToArray));
        static readonly MethodInfo EnumerableToListMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToList));

        static readonly LocalCache<int, MethodInfo> EnumerableSelectCache = new LocalCache<int, MethodInfo>();
        static readonly LocalCache<int, MethodInfo> EnumerableToArrayCache = new LocalCache<int, MethodInfo>();
        static readonly LocalCache<int, MethodInfo> EnumerableToListCache = new LocalCache<int, MethodInfo>();

        public static MethodInfo GetEnumerableSelect<TSource, TResult>()
        {
            var key = typeof(TSource).GetHashCode() ^ typeof(TResult).GetHashCode() ^ typeof(TSource).AssemblyQualifiedName.GetHashCode();
            return EnumerableSelectCache.Get(key, _ =>
             EnumerableSelectMethod.MakeGenericMethod(typeof(TSource), typeof(TResult)));
        }
        public static MethodInfo GetAsQueryable<TElement>()
        {
            return GetAsQueryable(typeof(TElement));
        }
        public static MethodInfo GetAsQueryable(Type elementType)
        {
            return AsQueryableMethod.MakeGenericMethod(elementType);
        }
        public static MethodInfo GetQuerybleSelect<TSource, TResult>()
        {
            return QuerySelectMethod.MakeGenericMethod(typeof(TSource), typeof(TResult));
        }

        public static MethodInfo GetEnumerableToArray<TResult>()
        {
            return EnumerableToArrayCache.Get(typeof(TResult).GetHashCode(),
                _ => EnumerableToArrayMethod.MakeGenericMethod(typeof(TResult)));
        }

        public static MethodInfo GetEnumerableToList<TResult>()
        {
            return EnumerableToListCache.Get(typeof(TResult).GetHashCode(),
                _ => EnumerableToListMethod.MakeGenericMethod(typeof(TResult)));
        }




    }
}
