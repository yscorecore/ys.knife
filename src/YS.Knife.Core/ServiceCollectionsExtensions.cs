using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace YS.Knife
{
    public static class ServiceCollectionsExtensions
    {
        public static IServiceCollection AddKnifeOptions<T>(this IServiceCollection services, IConfiguration configuration)
            where T : class
        {
            services.AddOptions<T>().Bind(configuration).ValidateDataAnnotations();
            services.AddSingleton<T>(sp => sp.GetRequiredService<IOptions<T>>().Value);
            return services;
        }

        public static IServiceCollection RegisterKnifeServices(this IServiceCollection services, IRegisterContext context)
        {
            foreach (var loaderType in AppDomain.CurrentDomain.FindInstanceTypesByBaseType<IServiceRegister>())
            {
                var loader = Activator.CreateInstance(loaderType) as IServiceRegister;
                loader?.RegisterServices(services, context);
            }
            return services;
        }
        public static IServiceCollection RegisterKnifeServices(this IServiceCollection services, IConfiguration configuration, ILogger logger = null, Func<Type, bool> typeFilter = null)
        {
            return services.RegisterKnifeServices(new RegisterContext
            {
                Configuration = configuration,
                Logger = logger,
                TypeFilter = typeFilter
            });
        }

        class RegisterContext : IRegisterContext
        {
            public IServiceCollection Services { get; set; }
            public IConfiguration Configuration { get; set; }
            public Func<Type, bool> TypeFilter { get; set; }
            public ILogger Logger { get; set; }
        }

    }
}
