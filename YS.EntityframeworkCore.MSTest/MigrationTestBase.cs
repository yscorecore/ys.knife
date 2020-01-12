using Knife.Hosting.MSTest;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace YS.EntityframeworkCore.MSTest
{

    public class MigrationTestBase<ContextType> : TestBase<ContextType>
        where ContextType : DbContext
    {
        public virtual TestContext TestContext { get; set; }

        [TestMethod]
        public async Task ApplyUpStepsOneByOne()
        {
            var migrator = this.TestObject.GetInfrastructure().GetService<IMigrator>();

            var migrationList = this.TestObject.Database.GetMigrations();
            Trace.TraceInformation($"===Ensure delete old database.");
            await this.TestObject.Database.EnsureDeletedAsync();

            foreach (var migration in migrationList)
            {
                Trace.TraceInformation($"==Apply up migration \"{migration}\"");
                await migrator.MigrateAsync(migration);
            }
        }
        [TestMethod]
        public async Task ApplyDownStepsOneByOne()
        {
            var migrator = this.TestObject.GetInfrastructure().GetService<IMigrator>();

            var migrationList = this.TestObject.Database.GetMigrations().Reverse();

            Trace.TraceInformation($"===Ensure delete old database.");
            await this.TestObject.Database.EnsureDeletedAsync();

            Trace.TraceInformation($"===Ensure creat new database.");
            await migrator.MigrateAsync((string)null);

            foreach (var migration in migrationList)
            {
                Trace.TraceInformation($"===Apply down migration \"{migration}\"");
                await migrator.MigrateAsync(migration);
            }

            await migrator.MigrateAsync("0");//delete all the things that migration created
        }

    }
}
