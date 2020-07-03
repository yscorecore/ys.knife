using System;
using AspectCore.Extensions.DependencyInjection;
namespace Microsoft.Extensions.Hosting
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseAopServiceProviderFactory(this IHostBuilder hostBuilder)
        {
            _ = hostBuilder ?? throw new ArgumentNullException(nameof(hostBuilder));
            hostBuilder.UseServiceProviderFactory((buildContext) =>
            {
                //return new DefaultServiceProviderFactory();
                return new DynamicProxyServiceProviderFactory();
            });
            return hostBuilder;
        }
    }
}
