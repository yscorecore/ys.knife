using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace YS.Knife.EntityFrameworkCore
{
    public abstract class DbContextConsumer<TContext> : IDbContextConsumer<TContext>
        where TContext : DbContext
    {
        public DbContextConsumer(TContext dbContext)
        {
            this.DbContext = dbContext;
        }
        public DbContext DbContext { get; }
    }
}
