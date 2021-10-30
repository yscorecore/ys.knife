using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace YS.Knife.EntityFrameworkCore
{
    public class EFEntityStore<TEntity, TContext> : IEntityStore<TEntity>, IDbContextConsumer
        where TContext : DbContext
        where TEntity : class
    {
        public EFEntityStore(TContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));
            this.Context = context;
            this.Set = context.Set<TEntity>();
        }

        public EFEntityStore()
        {
        }

        public TContext Context
        {
            get;
            private set;
        }
        public DbSet<TEntity> Set
        {
            get;
            private set;
        }

        DbContext IDbContextConsumer.DbContext => this.Context;

        public virtual void Add(TEntity entity)
        {
            this.Set.Add(entity);
        }


        public virtual void Delete(TEntity entity)
        {

            this.Set.Attach(entity);
            this.Context.Entry(entity).State = EntityState.Deleted;
        }

        public virtual TEntity FindByKey(params object[] keyValues)
        {
            return this.Set.Find(keyValues);
        }





        public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> conditions)
        {
            IQueryable<TEntity> query = this.Set;
            return conditions == null ?
            query :
            query.Where(conditions);

        }


        public virtual void Update(TEntity entity, params string[] fields)
        {
            if (fields == null || fields.Length == 0) return;
            var entry = this.Set.Attach(entity);
            foreach (var v in fields)
            {
                entry.Property(v).IsModified = true;
            }
        }

        public virtual int SaveChanges()
        {
            return this.Context.SaveChanges();
        }


        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return this.Context.SaveChangesAsync(cancellationToken);
        }
    }
}
