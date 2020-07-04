using System;
using Amazon.Extensions.Configuration.SystemsManager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using YS.Knife.Hosting;

namespace App
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var host = new AppHost(args))
            {
                host.Run();
            }
        }

        class AppHost : KnifeHost
        {
            public AppHost(string[] args) : base(args)
            {
            }
            protected override void OnConfigureAppConfiguration(HostBuilderContext hostBuilderContext, IConfigurationBuilder configurationBuilder)
            {
                AddSystemManager(hostBuilderContext, configurationBuilder);
                AddSecretManager(hostBuilderContext, configurationBuilder);
            }

            private void AddSecretManager(HostBuilderContext hostBuilderContext, IConfigurationBuilder configurationBuilder)
            {
                configurationBuilder.AddSystemsManager(
                    source =>
                    {
                        source.Path = "/aws/reference/secretsmanager/ypbapp";
                        source.Prefix = string.Empty;
                        source.Optional = true;
                        source.ReloadAfter = TimeSpan.FromSeconds(10);
                    });
            }
            private void AddSystemManager(HostBuilderContext hostBuilderContext, IConfigurationBuilder configurationBuilder)
            {
                configurationBuilder.AddSystemsManager("/ypbapp", true, TimeSpan.FromSeconds(10));
            }
        }
    }
}
