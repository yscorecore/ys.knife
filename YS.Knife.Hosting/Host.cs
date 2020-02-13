using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Knife.Hosting
{
    public static class Host
    {
        const string VerbRun = "run";
        public static void Run(string[] args)
        {
            Run(args, CreateHost);
        }
        public static void Run(string[] args, Func<string[], IHost> hostBuilder)
        {
            var argInfo = ParseArguments(args);
            using (var host = hostBuilder(args))
            {
                if (string.Equals(argInfo.Verb, VerbRun, StringComparison.InvariantCultureIgnoreCase))
                {
                    host.Run();
                }
                else
                {
                   RunStageVerb(host, argInfo.Verb);
                }
            }
        }

        private static (string Verb, string[] Arguments) ParseArguments(string[] args)
        {
            if (args.Length == 0)
            {
                return (VerbRun, Array.Empty<string>());
            }
            if (Regex.IsMatch(args[0], "\\w+"))
            {
                return (args[0].ToLower(), args.SubArray(1));
            }
            else
            {
                throw new ArgumentException($"The first argument should be a verb.(eg. run)");
            }

        }
        public static IHost CreateHost(string[] args = default)
        {
            return CreateHostBuilder(args).Build();
        }

        public static IHostBuilder CreateHostBuilder(string[] args = default)
        {
            return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args).ConfigureServices((builder, serviceCollection) =>
            {
                var options = builder.Configuration.GetConfigOrNew<AppOptions>();

                string rootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                var plugins = PluginLoader.LoadPlugins(rootPath, options.Plugins);

                serviceCollection.RegisteServices(plugins, builder.Configuration);

                serviceCollection.RegisteOptions(plugins, builder.Configuration);

            });

        }
        private static void RunStageVerb(IHost host, string name)
        {
            using (var scope = host.Services.CreateScope())
            {
                var handlers = scope.ServiceProvider.GetRequiredService<IEnumerable<IStageService>>().Where(p => string.Equals(name, p.StageName, StringComparison.InvariantCultureIgnoreCase)).ToList();
                ILogger logger = scope.ServiceProvider.GetRequiredService<ILogger<IHost>>();
                logger.LogInformation($"There {handlers.Count} handlers in {name} stage.");
                for (int i = 0; i < handlers.Count; i++)
                {
                    var index = i + 1;
                    var handler = handlers[i];
                    logger.LogInformation($"[{index:d2}]] Start exec handler {handler.GetType().Name}.");
                    handler.Run(CancellationToken.None).Wait();
                }
            }
        }
    }
}
