using System;
using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.Configuration;
namespace YS.Knife
{
    public static class ConfigurationExtensions
    {
        public static T GetConfig<T>(this IConfiguration configuration)
            where T : class

        {
            var configAttr = typeof(T).GetCustomAttribute<OptionsClassAttribute>();
            if (configAttr == null)
            {
                throw new InvalidOperationException($"Can not find {nameof(OptionsClassAttribute)} in type {typeof(T).FullName}.");
            }
            var optionsSection = configuration.GetOptionsConfiguration<T>(configAttr.ConfigKey);
            return optionsSection?.Get<T>();
        }

        public static IConfiguration GetOptionsConfiguration<T>(this IConfiguration configuration, string path)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (path == null)
            {
                var configKey = typeof(T).Name;
                if (configKey != "Options" && configKey.EndsWith("Options", true, CultureInfo.InvariantCulture))
                {
                    configKey = configKey.Substring(0, configKey.Length - 7);
                }
                return configuration.GetSection(configKey);
            }
            else if (path.Length == 0)
            {
                return configuration;

            }
            else
            {
                path = path.Replace('.', ':').Replace("__", ":");
                return configuration.GetSection(path);
            }
        }

        public static T GetConfigOrNew<T>(this IConfiguration configuration)
            where T : class, new()
        {
            return configuration.GetConfig<T>() ?? new T();
        }
    }
}
