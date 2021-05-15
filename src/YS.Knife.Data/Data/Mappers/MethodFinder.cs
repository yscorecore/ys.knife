using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace YS.Knife.Data.Mappers
{
    class MethodFinder
    {
        static MethodInfo enumerableSelectMethod = typeof(Enumerable).GetMethods()
              .Where(p => p.Name == nameof(Enumerable.Select))
              .Where(p => p.GetParameters().Length == 2)
              .Where(p => p.GetParameters().Last().ParameterType.GetGenericTypeDefinition() == typeof(Func<,>)).Single();

        static MethodInfo querySelectMethod = typeof(Queryable).GetMethods()
             .Where(p => p.Name == nameof(Queryable.Select))
             .Where(p => p.GetParameters().Length == 2)
             .Where(p => p.GetParameters().Last().ParameterType.GetGenericArguments().First().GetGenericTypeDefinition() == typeof(Func<,>)).Single();

        static MethodInfo enumerableToArrayMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToArray));
        static MethodInfo enumerableToListMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToList));
        public static MethodInfo GetEnumerableSelect<TSource, TResult>()
        {
            return enumerableSelectMethod.MakeGenericMethod(typeof(TSource), typeof(TResult));
        }
        public static MethodInfo GetQuerybleSelect<TSource, TResult>()
        {
            return querySelectMethod.MakeGenericMethod(typeof(TSource), typeof(TResult));
        }
        public static MethodInfo GetEnumerableToArray<TResult>()
        {
            return enumerableToArrayMethod.MakeGenericMethod(typeof(TResult));
        }
        public static MethodInfo GetEnumerableToList<TResult>()
        {
            return enumerableToListMethod.MakeGenericMethod(typeof(TResult));
        }
    }

}
