using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
namespace YS.Knife
{
    public static class ServiceCollectionsExtensions
    {
        public static IServiceCollection RegisteKnifeServices(this IServiceCollection services, IConfiguration configuration)
        {
            foreach (var loaderType in AppDomain.CurrentDomain.FindInstanceTypesByBaseType<IServiceLoader>())
            {
                var loader = Activator.CreateInstance(loaderType) as IServiceLoader;
                loader.LoadServices(services, configuration);
            }
            return services;
        }
    }
}