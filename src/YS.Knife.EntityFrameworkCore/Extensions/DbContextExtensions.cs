using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
namespace Microsoft.EntityFrameworkCore
{
    public static class DbContextExtensions
    {
        public static async Task MigrateUpByStepAsync(this DbContext dbContext)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }
            var migrator = dbContext.GetInfrastructure().GetService<IMigrator>();
            await dbContext.Database.EnsureDeletedAsync();

            foreach (var migration in dbContext.Database.GetMigrations())
            {
                await migrator.MigrateAsync(migration);
            }
        }
        public static void MigrateUpByStep(this DbContext dbContext)
        {
            dbContext.MigrateUpByStepAsync().Wait();
        }
        public static async Task MigrateDownByStepAsync(this DbContext dbContext)
        {
            _ = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            var migrator = dbContext.GetInfrastructure().GetService<IMigrator>();

            await dbContext.Database.EnsureDeletedAsync();

            await migrator.MigrateAsync(null);

            foreach (var migration in dbContext.Database.GetMigrations().Reverse())
            {
                await migrator.MigrateAsync(migration);
            }

            await migrator.MigrateAsync("0");//delete all the things that migration created
        }
        public static void MigrateDownByStep(this DbContext dbContext)
        {
            dbContext.MigrateDownByStepAsync().Wait();
        }

    }
}
