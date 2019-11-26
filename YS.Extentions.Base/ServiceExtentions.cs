using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System
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
                               where Attribute.IsDefined(p, typeof(ServiceImplClassAttribute))
                                     && !p.IsAbstract
                               select p;
            foreach (var serviceType in serviceTypes)
            {
                var serviceAttr = Attribute.GetCustomAttribute(serviceType, typeof(ServiceImplClassAttribute)) as ServiceImplClassAttribute;

                switch (serviceAttr.Lifetime)
                {
                    case ServiceLifetime.Singleton:
                        services.AddSingleton(serviceAttr.InjectType, serviceType);
                        break;
                    case ServiceLifetime.Scoped:
                        services.AddScoped(serviceAttr.InjectType, serviceType);
                        break;
                    case ServiceLifetime.Transient:
                        services.AddTransient(serviceAttr.InjectType, serviceType);
                        break;
                }
            }
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

        #region Options
        public static IServiceCollection RegisteOptions(this IServiceCollection services, Assembly assembly, IConfiguration configuration)
        {
            var configTypes = from p in assembly.GetTypes()
                              where Attribute.IsDefined(p, typeof(ConfigClassAttribute))
                                    && !p.IsAbstract
                              select p;
            foreach (var configType in configTypes)
            {
                var configAttr = Attribute.GetCustomAttribute(configType, typeof(ConfigClassAttribute)) as ConfigClassAttribute;
                var section = configuration.GetSection(configAttr.ConfigKey);
                AddOptionInternal(services, configType, section);
            }
            return services;
        }
        public static IServiceCollection RegisteOptions(this IServiceCollection services, IEnumerable<Assembly> assemblies, IConfiguration configuration)
        {
            foreach (var assembly in assemblies ?? Enumerable.Empty<Assembly>())
            {
                RegisteOptions(services, assembly, configuration);
            }
            return services;
        }

        private static void AddOptionInternal(IServiceCollection services, Type optionType, IConfiguration configuration)
        {
            var instance = Activator.CreateInstance(typeof(ConfigOptionProxy<>).MakeGenericType(optionType)) as IConfigOptionProxy;
            instance.Configure(services, configuration);

        }

        private interface IConfigOptionProxy
        {
            void Configure(IServiceCollection services, IConfiguration configuration);
        }
        private class ConfigOptionProxy<T> : IConfigOptionProxy
            where T : class
        {
            public void Configure(IServiceCollection services, IConfiguration configuration)
            {
                services.Configure<T>(configuration);
            }
        }
        #endregion
    }
}
