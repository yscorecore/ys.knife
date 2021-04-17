using System.Linq;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace YS.Knife
{
    public interface IQueryableMapper
    {
        private static IQueryableMapper instance;
        static IQueryableMapper Instance
        {
            get
            {
                if (instance is null)
                {
                    throw new InvalidOperationException($"Before use the static property '{nameof(Instance)}' in the type '{typeof(IQueryableMapper).FullName}', you should set the instance value first. ");
                }
                return instance;
            }
            set
            {
                instance = value ?? throw new ArgumentNullException(nameof(value));
            }
        }
        IQueryable<R> MapType<T, R>(IQueryable<T> from);
    }
    public static class IQueryableMapperExtenstions
    {
        private static MethodInfo methodInfo = typeof(IQueryableMapperExtenstions).GetMethod(nameof(MapType), BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        public static IQueryable<R> MapType<T, R>(this IQueryable<T> from)
        {
            return IQueryableMapper.Instance.MapType<T, R>(from);
        }
        public static IQueryable<R> MapTo<R>(this IQueryable from)
        {
            var genericQueryableType = from.GetType().GetInterfaces().Where(p => p.IsGenericType && p.GetGenericTypeDefinition() == typeof(IQueryable<>)).FirstOrDefault();
            if (genericQueryableType == null)
            {
                throw new InvalidOperationException("Can not case IQueryable instance to IQueryable<>.");
            }
            IQueryableMapper mapper = IQueryableMapper.Instance;
            var exeMethod = methodInfo.MakeGenericMethod(genericQueryableType.GetGenericArguments().First(), typeof(R));
            return (IQueryable<R>)(exeMethod.Invoke(null, new[] { from }));
            // from.GetType().GetInterface()
            //  return IQueryableMapper.Instance.MapType<T, R>(from);
        }
    }
}
