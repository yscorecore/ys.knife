using System;
using System.Threading;
using System.Threading.Tasks;
using YS.Knife;
using YS.Knife.Stage;

namespace Microsoft.EntityFrameworkCore
{
    public class DbContextMigrationInstaller<T> : Installer
        where T : DbContext
    {
        public DbContextMigrationInstaller(T context)
        {
            this.Context = context;
        }
        protected T Context { get; private set; }
        public override Task Run(CancellationToken cancellationToken = default)
        {
            return this.Context.Database.MigrateAsync(cancellationToken);
        }
    }
}
