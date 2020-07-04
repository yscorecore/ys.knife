using System.Linq;
namespace Microsoft.Extensions.Configuration
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder InsertSourceBefore<T>(this IConfigurationBuilder configurationBuilder, IConfigurationSource configurationSource)
            where T : IConfigurationSource
        {
            var current = configurationBuilder.Sources.OfType<T>().FirstOrDefault();
            if (current == null)
            {
                configurationBuilder.Add(configurationSource);
            }
            else
            {
                var index = configurationBuilder.Sources.IndexOf(current);
                configurationBuilder.Sources.Insert(index, configurationSource);
            }
            return configurationBuilder;
        }
        public static IConfigurationBuilder InsertSourceAfter<T>(this IConfigurationBuilder configurationBuilder, IConfigurationSource configurationSource)
            where T : IConfigurationSource
        {
            var current = configurationBuilder.Sources.OfType<T>().LastOrDefault();
            if (current == null)
            {
                configurationBuilder.Add(configurationSource);
            }
            else
            {
                var index = configurationBuilder.Sources.IndexOf(current);
                configurationBuilder.Sources.Insert(index + 1, configurationSource);
            }
            return configurationBuilder;
        }
        public static IConfigurationBuilder AddRemoteConfigurationSource(this IConfigurationBuilder configurationBuilder, IConfigurationSource configurationSource)
        {
            return configurationBuilder.InsertSourceBefore<EnvironmentVariables.EnvironmentVariablesConfigurationSource>(configurationSource);
        }
    }
}
