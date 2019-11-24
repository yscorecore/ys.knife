using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;

namespace System
{
    public static class AssemblyServiceCollectionExtentions
    {
        public static Assembly AppendOptions(this Assembly assembly, IServiceCollection services,  IConfiguration configuration)
        {
            var configTypes = from p in assembly.GetTypes()
                              where Attribute.IsDefined(p, typeof(ConfigClassAttribute))
                                    && !p.IsAbstract
                              select p;
            foreach (var configType in configTypes)
            {
                var configAttr = Attribute.GetCustomAttribute(configType, typeof(ConfigClassAttribute)) as ConfigClassAttribute;
                var section = configuration.GetSection(configAttr.ConfigKey);
                AddOptionInternal(services, configType, section);
            }
            return assembly;
        }
        
        private static void AddOptionInternal(IServiceCollection services, Type optionType, IConfiguration configuration)
        {
            var instance = Activator.CreateInstance(typeof(ConfigOptionProxy<>).MakeGenericType(optionType)) as IConfigOptionProxy;
            instance.Configure(services, configuration);

        }



        private interface IConfigOptionProxy
        {
            void Configure(IServiceCollection services, IConfiguration configuration);
        }
        private class ConfigOptionProxy<T> : IConfigOptionProxy
            where T : class
        {
            public void Configure(IServiceCollection services, IConfiguration configuration)
            {
                services.Configure<T>(configuration);
            }
        }
    }
}
