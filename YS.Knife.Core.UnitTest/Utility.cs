using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife
{
    public class Utility
    {
        public static IServiceProvider BuildProvider()
        {
            return BuildProvider(new Dictionary<string, string>());
        }
        public static IServiceProvider BuildProvider(IDictionary<string, string> configurationValues)
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationValues).Build();
            return services.RegisteKnifeServices(configuration).BuildServiceProvider();
        }
    }
}
