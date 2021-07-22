using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;


namespace YS.Knife
{
    public static class ReflectionExtensions
    {
        public static IEnumerable<Type> FindInstanceTypesByAttribute<T>(this AppDomain appDomain,
            Func<Type, bool> filter = null)
            where T : Attribute
        {
            _ = appDomain ?? throw new ArgumentNullException(nameof(appDomain));


            foreach (var assembly in appDomain.GetAllAssembliesIgnoreSystem())
            {
                foreach (var type in FindInstanceTypesByAttribute<T>(assembly, filter))
                {
                    yield return type;
                }
            }
        }
        public static IEnumerable<Type> FindInstanceTypesByAttribute<T>(this Assembly assembly,
         Func<Type, bool> filter = null)
         where T : Attribute
        {
            _ = assembly ?? throw new ArgumentNullException(nameof(assembly));
            var customFilter = filter ?? (type => true);


            foreach (var type in assembly.GetTypes())
            {
                if (type.IsClass
                    && !type.IsAbstract
                    && Attribute.IsDefined(type, typeof(T), false)
                    && customFilter(type))
                {
                    yield return type;
                }
            }

        }
        public static IEnumerable<Type> FindInterfaceTypesByAttribute<T>(this AppDomain appDomain,
            Func<Type, bool> filter = null)
            where T : Attribute
        {
            _ = appDomain ?? throw new ArgumentNullException(nameof(appDomain));
            var customFilter = filter ?? (type => true);

            foreach (var assembly in appDomain.GetAllAssembliesIgnoreSystem())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsInterface
                        && Attribute.IsDefined(type, typeof(T), false)
                        && customFilter(type))
                    {
                        yield return type;
                    }
                }
            }
        }

        public static IEnumerable<Type> FindInstanceTypesByBaseType<TBase>(this AppDomain appDomain,
            Func<Type, bool> filter = null)
        {
            _ = appDomain ?? throw new ArgumentNullException(nameof(appDomain));
            foreach (var assembly in appDomain.GetAllAssembliesIgnoreSystem())
            {
                foreach (var type in FindInstanceTypesByBaseType<TBase>(assembly, filter))
                {
                    yield return type;
                }
            }
        }
        public static IEnumerable<Type> FindInstanceTypesByBaseType<TBase>(this Assembly assembly,
           Func<Type, bool> filter = null)
        {
            _ = assembly ?? throw new ArgumentNullException(nameof(assembly));
            var customFilter = filter ?? (type => true);
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsClass
                    && !type.IsAbstract
                    && typeof(TBase).IsAssignableFrom(type)
                    && customFilter(type))
                {
                    yield return type;
                }
            }
        }

        public static IEnumerable<Type> FindInstanceTypesByAttributeAndBaseType<TAttribute, TBase>(
            this AppDomain appDomain, Func<Type, bool> filter = null)
            where TAttribute : Attribute
        {
            _ = appDomain ?? throw new ArgumentNullException(nameof(appDomain));
            var customFilter = filter ?? (type => true);
            foreach (var assembly in appDomain.GetAllAssembliesIgnoreSystem())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass
                        && !type.IsAbstract
                        && Attribute.IsDefined(type, typeof(TAttribute), false)
                        && typeof(TBase).IsAssignableFrom(type)
                        && customFilter(type))
                    {
                        yield return type;
                    }
                }
            }
        }

        public static bool IsFromMicrosoft(this Assembly assembly)
        {
            var companyAttr = assembly.GetCustomAttribute<AssemblyCompanyAttribute>();
            return companyAttr?.Company == "Microsoft Corporation";
        }

        private static IEnumerable<Assembly> GetAllAssembliesIgnoreSystem(this AppDomain appDomain)
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            foreach (var assembly in appDomain.GetAssemblies())
            {
                if (assembly != entryAssembly && !IsFromMicrosoft(assembly))
                {
                    yield return assembly;
                }
            }

            yield return entryAssembly;
        }

        private static readonly LocalCache<Type, object> DefaultValueCache = new LocalCache<Type, object>();
        public static object DefaultValue(this Type type)
        {
            if (type.IsClass)
            {
                return null;
            }
            return DefaultValueCache.Get(type,
                 innerType => (Activator.CreateInstance(typeof(DefaultValueProxy<>).MakeGenericType(innerType)) as IDefaultValueProxy)?.GetDefault());
        }

        interface IDefaultValueProxy
        {
            object GetDefault();
        }

        class DefaultValueProxy<T> : IDefaultValueProxy
        {
            public object GetDefault()
            {
                return default(T);
            }
        }
    }
}
