﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace YS.Knife
{
    public static class ServiceExtentions
    {
        #region Services
        public static IServiceCollection RegisteServices(this IServiceCollection services, Assembly assembly, IConfiguration configuration)
        {
            RegisteAttributeServices(services, assembly, configuration);
            RegisteCustomeServices(services, assembly, configuration);
            return services;
        }
        public static IServiceCollection RegisteServices(this IServiceCollection services, IEnumerable<Assembly> assemblies, IConfiguration configuration)
        {
            foreach (var assembly in assemblies ?? Enumerable.Empty<Assembly>())
            {
                RegisteServices(services, assembly, configuration);
            }
            return services;
        }

        private static void RegisteAttributeServices(IServiceCollection services, Assembly assembly, IConfiguration configuration)
        {
            var serviceTypes = from p in assembly.GetTypes()
                               where Attribute.IsDefined(p, typeof(ServiceClassAttribute))
                                     && !p.IsAbstract
                               select p;
            foreach (var serviceType in serviceTypes)
            {
                var serviceAttr = Attribute.GetCustomAttribute(serviceType, typeof(ServiceClassAttribute)) as ServiceClassAttribute;
                var injectType = serviceAttr.InjectType ?? DeduceInjectType(serviceType);
                switch (serviceAttr.Lifetime)
                {
                    case ServiceLifetime.Singleton:
                        services.AddSingleton(injectType, serviceType);
                        break;
                    case ServiceLifetime.Scoped:
                        services.AddScoped(injectType, serviceType);
                        break;
                    case ServiceLifetime.Transient:
                        services.AddTransient(injectType, serviceType);
                        break;
                }
            }
        }
        private static Type DeduceInjectType(Type serviceType)
        {
            var allInterfaces = serviceType.GetInterfaces();
            if (allInterfaces.Length != 1)
            {
                throw new InvalidOperationException($"Can not deduce the inject type from current type '{serviceType.FullName}'.");
            }
            return allInterfaces.First();
        }
        private static void RegisteCustomeServices(IServiceCollection services, Assembly assembly, IConfiguration configuration)
        {
            var loadTypes = from p in assembly.GetTypes()
                            where typeof(IServiceLoader).IsAssignableFrom(p)
                                  && !p.IsAbstract && p.GetConstructor(Type.EmptyTypes) != null
                            select p;
            foreach (var loadType in loadTypes)
            {
                IServiceLoader loader = Activator.CreateInstance(loadType) as IServiceLoader;
                loader.LoadServices(services, configuration);
            }
        }

        #endregion



    }
}
