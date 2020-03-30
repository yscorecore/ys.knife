﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using YS.Knife.Stage;
namespace YS.Knife.Hosting
{

    public class KnifeHost : IDisposable, IServiceProvider
    {
        public KnifeHost() : this(Array.Empty<string>())
        {
        }
        public KnifeHost(IDictionary<string, object> args, Action<HostBuilderContext, IServiceCollection> configureDelegate = null)
            : this(args?.Select(p => $"/{p.Key}={p.Value}").ToArray(), configureDelegate)
        {
        }
        public KnifeHost(string[] args, Action<HostBuilderContext, IServiceCollection> configureDelegate = null)
        {
            this.args = args;
            this.configureDelegate = configureDelegate;
#pragma warning disable CA2214 // 不要在构造函数中调用可重写的方法
            this.host = CreateHostBuilder().Build();
#pragma warning restore CA2214 // 不要在构造函数中调用可重写的方法
        }

        private readonly Action<HostBuilderContext, IServiceCollection> configureDelegate;
        private readonly string[] args;
        private readonly IHost host;

        public object GetService(Type serviceType)
        {
            return host.Services.GetService(serviceType);
        }
        public T GetService<T>()
        {
            return ServiceProviderServiceExtensions.GetService<T>(this);
        }

        protected virtual IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder(args ?? Array.Empty<string>())
                .ConfigureServices((builder, serviceCollection) =>
                {
                    LoadKnifeServices(serviceCollection, builder.Configuration);
                    this.LoadCustomService(builder, serviceCollection);
                });

        }
        protected virtual void LoadCustomService(HostBuilderContext builder, IServiceCollection serviceCollection)
        {
            configureDelegate?.Invoke(builder, serviceCollection);
        }



        protected void RunStage(string name)
        {
            using (var scope = host.Services.CreateScope())
            {
                var handlers = scope.ServiceProvider.GetRequiredService<IEnumerable<IStageService>>().Where(p => string.Equals(name, p.StageName, StringComparison.InvariantCultureIgnoreCase)).ToList();
                ILogger logger = scope.ServiceProvider.GetRequiredService<ILogger<KnifeHost>>();
                logger.LogInformation($"There are {handlers.Count} handlers in {name} stage.");
                for (int i = 0; i < handlers.Count; i++)
                {
                    var index = i + 1;
                    var handler = handlers[i];
                    logger.LogInformation($"[{index:d2}] Start exec handler {handler.GetType().Name}.");
                    handler.Run(CancellationToken.None).Wait();
                }
            }
        }
        protected void RunDefault()
        {
            host.Run();
        }

        public void Run()
        {
            var options = this.host.Services.GetService<IOptions<KnifeOptions>>();
            if (IsDefaultVerb(options.Value.Stage))
            {
                RunDefault();
            }
            else
            {
                RunStage(options.Value.Stage);
            }
        }

        private static bool IsDefaultVerb(string stage)
        {
            return string.IsNullOrEmpty(stage) ||
                   string.Equals("default", stage, StringComparison.InvariantCultureIgnoreCase);
        }
        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    host.Dispose();
                }
                disposedValue = true;
            }
        }
        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Static

        public static void Start(string[] args, Action<HostBuilderContext, IServiceCollection> configureDelegate = null)
        {
            using (var knifeHost = new KnifeHost(args, configureDelegate))
            {
                knifeHost.Run();
            }
        }
        [SuppressMessage("Reliability", "IDE0067:丢失范围之前释放对象", Justification = "<挂起>")]
        [SuppressMessage("Reliability", "CA2000:丢失范围之前释放对象", Justification = "<挂起>")]
        public static void LoadKnifeServices(IServiceCollection services, IConfiguration configuration)
        {
            var serviceProvider = services.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>() ?? new LoggerFactory();
            var logger = loggerFactory.CreateLogger("Knife");
            var options = configuration.GetConfigOrNew<KnifeOptions>();

            PluginRegister.LoadPlugins(options.Plugins);
            var knifeTypeFilter = new KnifeTypeFilter(options);
            services.RegisteKnifeServices(configuration, logger, knifeTypeFilter.IsFilter);
        }



        #endregion
    }
}
