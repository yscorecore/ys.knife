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

            if (configAttr.ConfigKey == string.Empty)
            {
                return configuration.Get<T>();
            }
            else if (configAttr.ConfigKey == null)
            {
                var configKey = typeof(T).Name;
                if (configKey != "Options" && configKey.EndsWith("Options"))
                {
                    configKey = configKey.Substring(0, configKey.Length - 7);
                }
                var section = configuration.GetSection(configAttr.ConfigKey);
                return section.Get<T>();
            }
            else
            {
                var section = configuration.GetSection(configAttr.ConfigKey);
                return section.Get<T>();
            }
        }
        public static T GetConfigOrNew<T>(this IConfiguration configuration)
            where T : class, new()
        {
            return configuration.GetConfig<T>() ?? new T();
        }
    }
}
