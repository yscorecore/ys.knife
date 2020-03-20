using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife
{
    public class Utility
    {
        public static IServiceProvider BuildProvider(Action<IServiceCollection, IConfiguration> config = null)
        {
            return BuildProvider(new Dictionary<string, string>(), config);
        }
        public static IServiceProvider BuildProvider(IDictionary<string, string> configurationValues,Action<IServiceCollection,IConfiguration> config=null)
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationValues).Build();
            config?.Invoke(services, configuration);
            return services.RegisteKnifeServices(configuration).BuildServiceProvider();
        }
    }
}
