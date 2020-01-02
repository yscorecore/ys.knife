using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace System
{
    public abstract class DesignTimeDbContextFactory<T> : IDesignTimeDbContextFactory<T>
        where T : DbContext
    {

        public T CreateDbContext(string[] args)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            string envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(basePath)
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
            var configuration = configurationBuilder.Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var options = new DbContextOptionsBuilder<T>()
                    .UseSqlServer(connectionString)
                    .Options;
            return OnCreateDbContextInstance(options);
        }
        protected abstract T OnCreateDbContextInstance(DbContextOptions<T> options);

    }
}
