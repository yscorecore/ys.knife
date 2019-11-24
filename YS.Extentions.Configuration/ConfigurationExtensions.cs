using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public static class ConfigurationExtensions
    {
        public const string DBContextConfigsKey = "DBContextConfigs";
        public static DBContextConfig GetDbContextConfig(this IConfiguration configuration, string name, DBContextConfig defaultConfig)
        {
            IConfigurationSection section = configuration.GetSection(DBContextConfigsKey);
            if (section == null)
            {
                return defaultConfig;
            }
            IConfigurationSection configSection = section.GetSection(name);
            if (configuration == null)
            {
                return defaultConfig;
            }
            return configSection.Get<DBContextConfig>() ?? defaultConfig;
        }

        //public static T GetConfig<T>(this IConfiguration configuration)
        //    where T : class     
            
        //{
        //    var configAttr = Attribute.GetCustomAttribute(typeof(T), typeof(ConfigClassAttribute)) as ConfigClassAttribute;
        //    if (configAttr == null)
        //    {
        //        throw new InvalidOperationException($"类型“{typeof(T).FullName}”没有应用 “{nameof(ConfigClassAttribute)}”特性。");
        //    }
        //    var section = configuration.GetSection(configAttr.ConfigKey);

        //    return section.Get<T>();
        //}
        public static T GetConfigOrNew<T>(this IConfiguration configuration)
            where T : class,new()
        {
            return configuration.GetConfig<T>()??new T();
        }
    }
}
