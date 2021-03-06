﻿using System;
using System.Collections.Generic;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace YS.Knife
{
    public class Utility
    {
        public static IServiceProvider BuildProvider(Action<IServiceCollection, IConfiguration> config = null)
        {
            return BuildProvider(new Dictionary<string, string>(), config);
        }

        public static IServiceProvider BuildProvider(IDictionary<string, string> configurationValues, Action<IServiceCollection, IConfiguration> config = null)
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(configurationValues).Build();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
            config?.Invoke(services, configuration);
            return services.RegisterKnifeServices(configuration).BuildDynamicProxyProvider();
        }

        public static IHost BuildHost()
        {
            return Host.CreateDefaultBuilder(Array.Empty<string>())
                .ConfigureServices((builder, serviceCollection) =>
                {
                    serviceCollection.RegisterKnifeServices(builder.Configuration);
                }).Build();
        }
    }
}
