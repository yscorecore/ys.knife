using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using YS.Knife.EntityFrameworkCore;

namespace OneCms.EFCore
{

    [EFCoreContext]
    public class CmsContext : KnifeDbContext
    {
        public CmsContext(DbContextOptions<CmsContext> dbContextOptions, DbContextModelConfigration<CmsContext> dbModelConfigration) :
            base(dbContextOptions, dbModelConfigration)
        {

        }

        public virtual DbSet<Post> Posts { get; set; }

        public virtual DbSet<Topic> Topics { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            foreach (var type in AllEntityTypes())
            {
                modelBuilder.Entity(type).Property<string>(nameof(CmsBaseEntity.CreatedBy)).HasMaxLength(128);
                modelBuilder.Entity(type).Property<string>(nameof(CmsBaseEntity.UpdatedBy)).HasMaxLength(128);
            }
        }

        private IEnumerable<Type> AllEntityTypes()
        {
            return this.GetType().Assembly.GetTypes().Where(p => p.IsClass && !p.IsAbstract && typeof(CmsBaseEntity).IsAssignableFrom(p));

        }
    }


}
