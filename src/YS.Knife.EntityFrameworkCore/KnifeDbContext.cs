using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore
{
    public abstract class KnifeDbContext : DbContext
    {
        public DbContextModelConfigration ModelConfigration { get; }
        protected KnifeDbContext()
        {

        }
        protected KnifeDbContext(DbContextOptions dbContextOptions) : this(dbContextOptions, null)
        {

        }
        public KnifeDbContext(DbContextOptions dbContextOptions, DbContextModelConfigration modelConfigration) : base(dbContextOptions)
        {
            this.ModelConfigration = modelConfigration;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ModelConfigration?.ConfigModels(modelBuilder);
        }
    }
    public abstract class DbContextConfigration
    {
        public abstract void ConfigOptions(IServiceProvider sp, DbContextOptionsBuilder builder);
        
    }

    public abstract class DbContextConfigration<T> : DbContextConfigration
          where T : DbContext
    {
       
    }
    public abstract class DbContextModelConfigration
    {
        public virtual void ConfigModels(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }
    }

    public abstract class DbContextModelConfigration<T> : DbContextModelConfigration
        where T : DbContext
    {
      
    }


}
