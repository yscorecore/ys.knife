using System;
using YS.Knife.Extensions.Configuration.Api;

namespace Microsoft.Extensions.Configuration
{
    public static class ApiConfigurationExtensions
    {
        public static IConfigurationBuilder AddApiConfiguration(
            this IConfigurationBuilder builder, Action<ApiConfigurationSource> action)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            var source = new ApiConfigurationSource();
            action?.Invoke(source);
            return builder.Add(source);
        }
        public static IConfigurationBuilder AddApiConfiguration(this IConfigurationBuilder builder, string apiUrl)
        {
            return AddApiConfiguration(builder, (apiSource) =>
            {
                apiSource.ApiUrl = apiUrl;
            });
        }
    }
}
