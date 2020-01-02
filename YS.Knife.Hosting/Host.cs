using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Knife.Hosting
{
    public static class Host
    {
        public static void Run(string[] args)
        {
            using (var host = CreateHost(args))
            {
                host.Run();
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
    }
}
