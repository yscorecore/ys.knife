using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace YS.Knife
{
    public static class ServiceCollectionsExtensions
    {
        public static IServiceCollection RegisteKnifeServices(this IServiceCollection services, IRegisteContext context)
        {
            foreach (var loaderType in AppDomain.CurrentDomain.FindInstanceTypesByBaseType<IServiceRegister>())
            {
                var loader = Activator.CreateInstance(loaderType) as IServiceRegister;
                loader.RegisteServices(services, context);
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
