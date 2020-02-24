using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Linq;
namespace Microsoft.EntityFrameworkCore
{
    public static class DbContextExtensions
    {
        public static async Task MigrateUpByStepAsync(this DbContext dbContext)
        {
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
            var migrator = dbContext.GetInfrastructure().GetService<IMigrator>();

            await dbContext.Database.EnsureDeletedAsync();

            await migrator.MigrateAsync((string)null);

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
