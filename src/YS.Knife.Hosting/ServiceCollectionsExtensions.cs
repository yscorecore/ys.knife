﻿using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace YS.Knife.Hosting
{
    public static class ServiceCollectionsExtensions
    {
        [SuppressMessage("Reliability", "IDE0067:丢失范围之前释放对象", Justification = "<挂起>")]
        [SuppressMessage("Reliability", "CA2000:丢失范围之前释放对象", Justification = "<挂起>")]
        public static void AddAllKnifeServices(this IServiceCollection services, IConfiguration configuration)
        {
            _ = configuration ?? throw new ArgumentNullException(nameof(configuration));
            AddAllKnifeServices(services, configuration, null);
        }
        [SuppressMessage("Reliability", "IDE0067:丢失范围之前释放对象", Justification = "<挂起>")]
        [SuppressMessage("Reliability", "CA2000:丢失范围之前释放对象", Justification = "<挂起>")]
        public static void AddAllKnifeServices(this IServiceCollection services, IConfiguration configuration, Func<Type, bool> typeFilter)
        {
            _ = configuration ?? throw new ArgumentNullException(nameof(configuration));
            var serviceProvider = services.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>() ?? new LoggerFactory();
            var logger = loggerFactory.CreateLogger("Knife");
            var options = configuration.GetConfigOrNew<KnifeOptions>();

            PluginRegister.LoadPluginPaths(options.PluginPaths, logger);
            var knifeTypeFilter = new KnifeTypeFilter(options);
            services.RegisterKnifeServices(configuration, logger, (type) =>
            {
                return (typeFilter?.Invoke(type) ?? false) || knifeTypeFilter.IsFilter(type);
            });
        }
    }
}
