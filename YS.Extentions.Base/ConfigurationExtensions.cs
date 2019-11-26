using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public static class ConfigurationExtensions
    {
        

        public static T GetConfig<T>(this IConfiguration configuration)
            where T : class     
            
        {
            var configAttr = Attribute.GetCustomAttribute(typeof(T), typeof(ConfigClassAttribute)) as ConfigClassAttribute;
            if (configAttr == null)
            {
                throw new InvalidOperationException($"Can not find {nameof(ConfigClassAttribute)} in type {typeof(T).FullName}.");
            }
            var section = configuration.GetSection(configAttr.ConfigKey);

            return section.Get<T>();
        }
        public static T GetConfigOrNew<T>(this IConfiguration configuration)
            where T : class,new()
        {
            return configuration.GetConfig<T>()??new T();
        }
    }
}
