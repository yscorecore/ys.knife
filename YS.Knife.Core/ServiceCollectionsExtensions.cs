using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace YS.Knife
{
    public static class ServiceCollectionsExtensions
    {
        public static IServiceCollection RegisteKnifeServices(this IServiceCollection services, IRegisteContext context)
        {
            foreach (var loaderType in AppDomain.CurrentDomain.FindInstanceTypesByBaseType<IServiceLoader>())
            {
                var loader = Activator.CreateInstance(loaderType) as IServiceLoader;
                loader.LoadServices(services, context);
            }
            return services;
        }
        public static IServiceCollection RegisteKnifeServices(this IServiceCollection services, IConfiguration configuration, ILogger logger = null, Func<Type, bool> typeFilter = null)
        {
            return services.RegisteKnifeServices(new RegisteContext
            {
                Configuration = configuration,
                Logger = logger,
                TypeFilter = typeFilter
            });
        }

        class RegisteContext : IRegisteContext
        {
            public IServiceCollection Services { get; set; }
            public IConfiguration Configuration { get; set; }
            public Func<Type, bool> TypeFilter { get; set; }
            public ILogger Logger { get; set; }
        }

    }
}