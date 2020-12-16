using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using YS.Knife.Aop;

namespace YS.Knife.Mongo
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class TransactionAttribute : BaseAopAttribute
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));
            _ = next ?? throw new ArgumentNullException(nameof(next));
            var mongoContext = GetCurrentMongoContext(context);
            if (mongoContext == null)
            {
                var logger =
                    context.ServiceProvider.GetService(
                        typeof(ILogger<>).MakeGenericType(context.ImplementationMethod.DeclaringType)) as ILogger;
                logger.LogWarning($"Can not find mongo context in current type '{context.ImplementationMethod.DeclaringType}', {nameof(TransactionAttribute)} will be ignored.");
                await next?.Invoke(context);
                return;
            }

            bool started = false;
            try
            {
                started = StartTransaction(mongoContext);
                await next?.Invoke(context);
                if (started)
                {
                    CommitTransaction(mongoContext);
                }
            }
            catch
            {
                if (started)
                {
                    RollbackTransaction(mongoContext);
                }
                throw;
            }
            finally
            {
                if (started)
                {
                    ResetTransaction(mongoContext);
                }
            }
        }

        private MongoContext GetCurrentMongoContext(AspectContext context)
        {
            var mongoContexts = context.ImplementationMethod.DeclaringType.GetFields(BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic)
                .Select(p => p.GetValue(context.Implementation))
                .Where(p => p is MongoContext || p is IMongoContextConsumer)
                .Select(p => p is MongoContext ? p : (p as IMongoContextConsumer).MongoContext)
                .OfType<MongoContext>()
                .Distinct()
                .ToList();
            if (mongoContexts.Count > 1)
            {
                throw new NotSupportedException($"Can't deduce mongo context, there are too many mongo context fields in '{context.ImplementationMethod.DeclaringType}' type, please start transaction manually.");
            }

            return mongoContexts.FirstOrDefault();
        }

        private bool StartTransaction(MongoContext mongoContext)
        {
            if (mongoContext.Session == null)
            {
                mongoContext.Session = mongoContext.Client.StartSession();
                mongoContext.Session.StartTransaction();
                return true;
            }
            return false;
        }

        private void CommitTransaction(MongoContext _mongoContext)
        {
            _mongoContext.Session?.CommitTransaction();
        }

        private void RollbackTransaction(MongoContext mongoContext)
        {
            mongoContext.Session?.AbortTransaction();
        }

        private void ResetTransaction(MongoContext mongoContext)
        {
            mongoContext.Session = null;
        }
    }




}
