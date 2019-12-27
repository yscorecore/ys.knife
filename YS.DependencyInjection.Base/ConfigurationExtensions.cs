using Microsoft.Extensions.Configuration;

namespace System
{
    public static class ConfigurationExtensions
    {
        public static T GetConfig<T>(this IConfiguration configuration)
            where T : class

        {
            var configAttr = Attribute.GetCustomAttribute(typeof(T), typeof(OptionsClassAttribute)) as OptionsClassAttribute;
            if (configAttr == null)
            {
                throw new InvalidOperationException($"Can not find {nameof(OptionsClassAttribute)} in type {typeof(T).FullName}.");
            }
            var optionsSection = configuration.GetOptionsConfiguration<T>(configAttr.ConfigKey);
            return optionsSection == null ? null : optionsSection.Get<T>();
        }

        public static IConfiguration GetOptionsConfiguration<T>(this IConfiguration configuration, string path)
        {
            if (path == string.Empty)
            {
                return configuration;
            }
            else if (path == null)
            {
                var configKey = typeof(T).Name;
                if (configKey != "Options" && configKey.EndsWith("Options"))
                {
                    configKey = configKey.Substring(0, configKey.Length - 7);
                }
                return configuration.GetSection(configKey);
            }
            else
            {
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
