using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using YS.Knife;

namespace Microsoft.EntityFrameworkCore
{
    public abstract class DesignTimeDbContextFactoryBase<T> : IDesignTimeDbContextFactory<T>
       where T : DbContext
    {
        public virtual T CreateDbContext(string[] args)
        {
            var serviceProvider = BuildProvider(args);
            return serviceProvider.GetRequiredService<T>();
        }
        private IServiceProvider BuildProvider(string[] args)
        {
            var services = new ServiceCollection();
            var configuration = CreateConfiguration(args);
            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            return services.RegisterKnifeServices(configuration).BuildServiceProvider();
        }
        private IConfiguration CreateConfiguration(string[] args)
        {

            string envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath((string)Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, false);

            if (!string.IsNullOrEmpty(envName))
            {
                configurationBuilder.AddJsonFile($"appsettings.{envName}.json", true, false);
            }

            configurationBuilder.AddEnvironmentVariables();

            if (args != null)
            {
                configurationBuilder.AddCommandLine(args);
            }

            return configurationBuilder.Build();
        }
    }
}
