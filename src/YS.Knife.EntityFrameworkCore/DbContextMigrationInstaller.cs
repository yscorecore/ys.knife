using System.Threading;
using System.Threading.Tasks;
using YS.Knife.Stages;

namespace Microsoft.EntityFrameworkCore
{
    [Stage("install")]
    public class DbContextMigrationInstaller<T> : IStageService
        where T : DbContext
    {
        public DbContextMigrationInstaller(T context)
        {
            this.Context = context;
        }
        protected T Context { get; private set; }
        public Task Run(CancellationToken cancellationToken = default)
        {
            return this.Context.Database.MigrateAsync(cancellationToken);
        }
    }
}
