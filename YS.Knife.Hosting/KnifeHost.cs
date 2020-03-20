using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace YS.Knife.Hosting
{
    public class KnifeHost : IDisposable, IServiceProvider
    {
        public KnifeHost() : this(new string[0])
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
            this.host = CreateHostBuilder().Build();
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
            return Host.CreateDefaultBuilder(args ?? new string[0])
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

        private bool IsDefaultVerb(string stage)
        {
            return string.IsNullOrEmpty(stage) ||
                string.Equals("default", stage, StringComparison.InvariantCultureIgnoreCase);
        }

        void IDisposable.Dispose()
        {
            this.host.Dispose();
        }
        #region Static

        public static void Start(string[] args, Action<HostBuilderContext, IServiceCollection> configureDelegate = null)
        {
            new KnifeHost(args, configureDelegate).Run();
        }
        public static void LoadKnifeServices(IServiceCollection services, IConfiguration configuration)
        {
            var serviceProvider = services.BuildServiceProvider();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>() ?? new LoggerFactory();
            var logger = loggerFactory.CreateLogger("Knife");
            var options = configuration.GetConfigOrNew<KnifeOptions>();

            PluginLoader.LoadPlugins(options.Plugins);
            var knifeTypeFilter = new KnifeTypeFilter(options);
            services.RegisteKnifeServices(configuration, logger, knifeTypeFilter.IsFilter);
        }

        #endregion
    }
}
