﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace YS.Knife
{
    public static class ReflectionExtensions
    {
        public static IEnumerable<Type> FindInstanceTypesByAttribute<T>(this AppDomain appDomain,
            Func<Type, bool> filter = null)
            where T : Attribute
        {
            _ = appDomain ?? throw new ArgumentNullException(nameof(appDomain));
            var customFilter = filter ?? (type => true);

            foreach (var assembly in appDomain.GetAllAssembliesIgnoreSystem())
            {
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
        }

        public static IEnumerable<Type> FindInstanceTypesByBaseType<TBase>(this AppDomain appDomain,
            Func<Type, bool> filter = null)
        {
            _ = appDomain ?? throw new ArgumentNullException(nameof(appDomain));
            var customFilter = filter ?? (type => true);
            foreach (var assembly in appDomain.GetAllAssembliesIgnoreSystem())
            {
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
        }

        public static IEnumerable<Type> FindInstanceTypesByAttributeAndBaseType<TAttrbute, TBase>(
            this AppDomain appDomain, Func<Type, bool> filter = null)
            where TAttrbute : Attribute
        {
            _ = appDomain ?? throw new ArgumentNullException(nameof(appDomain));
            var customFilter = filter ?? (type => true);
            foreach (var assembly in appDomain.GetAllAssembliesIgnoreSystem())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass
                        && !type.IsAbstract
                        && Attribute.IsDefined(type, typeof(TAttrbute), false)
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
    }
}