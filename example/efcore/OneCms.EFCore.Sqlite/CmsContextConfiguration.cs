using System;
using Microsoft.EntityFrameworkCore;
using YS.Knife;
using YS.Knife.EntityFrameworkCore;

namespace OneCms.EFCore.Sqlite
{
    [Service]
    public class CmsContextConfiguration : DbContextConfigration<CmsContext>
    {
        public override void ConfigOptions(IServiceProvider sp, DbContextOptionsBuilder builder)
        {
            builder.UseSqlite("Data Source=cms.db", (sqliteBuilder) =>
            {
                sqliteBuilder.MigrationsAssembly(this.GetType().Assembly.FullName);
            });
        }
    }
}
