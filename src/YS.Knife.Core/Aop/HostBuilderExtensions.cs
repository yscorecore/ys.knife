using System;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseAopServiceProviderFactory(this IHostBuilder hostBuilder)
        {
            _ = hostBuilder ?? throw new ArgumentNullException(nameof(hostBuilder));
            return hostBuilder.UseServiceProviderFactory((buildContext) => new DynamicProxyServiceProviderFactoryWithOptions());
        }
        public static IHostBuilder UseAopServiceProviderFactory(this IHostBuilder hostBuilder, bool validateScopes)
        {
            _ = hostBuilder ?? throw new ArgumentNullException(nameof(hostBuilder));
            return hostBuilder.UseServiceProviderFactory((buildContext) => new DynamicProxyServiceProviderFactoryWithOptions(validateScopes));
        }
        public static IHostBuilder UseAopServiceProviderFactory(this IHostBuilder hostBuilder, ServiceProviderOptions serviceProviderOptions)
        {
            _ = hostBuilder ?? throw new ArgumentNullException(nameof(hostBuilder));
            return hostBuilder.UseServiceProviderFactory((buildContext) => new DynamicProxyServiceProviderFactoryWithOptions(serviceProviderOptions));
        }
        private class DynamicProxyServiceProviderFactoryWithOptions : IServiceProviderFactory<IServiceCollection>
        {
            public DynamicProxyServiceProviderFactoryWithOptions()
            {
            }

            public DynamicProxyServiceProviderFactoryWithOptions(bool validateScopes) : this(new ServiceProviderOptions
            {
                ValidateScopes = validateScopes
            })
            {
            }

            public DynamicProxyServiceProviderFactoryWithOptions(ServiceProviderOptions serviceProviderOptions)
            {
                ServiceProviderOptions = serviceProviderOptions;
            }

            public ServiceProviderOptions ServiceProviderOptions { get; }

            public IServiceCollection CreateBuilder(IServiceCollection services)
            {
                return services;
            }

            public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
            {
                return ServiceProviderOptions is null
                    ? containerBuilder.BuildDynamicProxyProvider()
                    : containerBuilder.BuildDynamicProxyProvider(ServiceProviderOptions);
            }
        }
    }
}
