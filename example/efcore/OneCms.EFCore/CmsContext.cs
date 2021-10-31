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

    [KnifeEFContext]
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
            //modelBuilder.Entity<Topic>().Property<string>("_tenantId").HasColumnName("TenantId");

            // Configure entity filters
            #region FilterConfiguration
            //modelBuilder.Entity<Topic>().HasQueryFilter(b => EF.Property<string>(b, "_tenantId") == "00001");
            #endregion
        }
        public override int SaveChanges()
        {
            //ChangeTracker.DetectChanges();

            //foreach (var item in ChangeTracker.Entries().Where(
            //    e =>
            //        e.State == EntityState.Added && e.Metadata.GetProperties().Any(p => p.Name == "_tenantId")))
            //{
            //    item.CurrentValues["_tenantId"] = "00001";
            //}
            return base.SaveChanges();
        }
    }


}
