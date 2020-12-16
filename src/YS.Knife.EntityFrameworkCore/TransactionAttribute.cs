using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YS.Knife.Aop;

namespace YS.Knife.EntityFrameworkCore
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class TransactionAttribute : BaseAopAttribute
    {
        public TransactionAttribute()
        {
            this.Order = 10000;
        }

        public  override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));
            _ = next ?? throw new ArgumentNullException(nameof(next));
            var dbContext = GetCurrentDbContext(context);
            if (dbContext == null)
            {
                var logger =
                    context.ServiceProvider.GetService(
                        typeof(ILogger<>).MakeGenericType(context.ImplementationMethod.DeclaringType)) as ILogger;
                logger.LogWarning(
                    $"Can not find db context in current type '{context.ImplementationMethod.DeclaringType}', {nameof(TransactionAttribute)} will be ignored.");
                await next?.Invoke(context);
                return;
            }

            bool started = false;
            try
            {
                started = StartTransaction(dbContext);
                await next?.Invoke(context);
                await dbContext.SaveChangesAsync();
                if (started)
                {
                    CommitTransaction(dbContext);
                }
            }
            catch
            {
                if (started)
                {
                    RollbackTransaction(dbContext);
                }

                throw;
            }
            finally
            {
                if (started)
                {
                    ResetTransaction(dbContext);
                }
            }
        }

        private DbContext GetCurrentDbContext(AspectContext context)
        {
            var dbContexts = context.ImplementationMethod.DeclaringType.GetFields(BindingFlags.Instance |
                                                                                  BindingFlags.Public |
                                                                                  BindingFlags.NonPublic)
                .Select(p => p.GetValue(context.Implementation))
                .Where(p => p is DbContext || p is IDbContextConsumer)
                .Select(p => p is DbContext ? p : (p as IDbContextConsumer).DbContext)
                .OfType<DbContext>()
                .Distinct()
                .ToList();
            if (dbContexts.Count > 1)
            {
                throw new NotSupportedException(
                    $"Can't deduce db context, there are too many db context fields in '{context.ImplementationMethod.DeclaringType}' type, please start transaction manually.");
            }

            return dbContexts.FirstOrDefault();
        }

        private bool StartTransaction(DbContext dbContext)
        {
            if (dbContext.Database.AutoTransactionsEnabled)
            {
                return false;
            }

            if (dbContext.Database.CurrentTransaction == null)
            {
                dbContext.Database.BeginTransaction();
                return true;
            }

            return false;
        }

        private void CommitTransaction(DbContext dbContext)
        {
            dbContext.Database.CurrentTransaction?.Commit();
        }

        private void RollbackTransaction(DbContext dbContext)
        {
            dbContext.Database.CurrentTransaction?.Rollback();
        }

        private void ResetTransaction(DbContext dbContext)
        {
        }
    }
}
