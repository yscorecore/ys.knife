using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using YS.Knife.Aop;
using MongoDB.Driver;

namespace YS.Knife.Mongo
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class TransactionAttribute : BaseAopAttribute
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));
            _ = next ?? throw new ArgumentNullException(nameof(next));
            var mongoContexts = context.ServiceProvider.GetService<IEnumerable<MongoContext>>()
                .Where(p => p.Transaction == null).ToList();
            List<MongoContext> successContexts = new List<MongoContext>();
            try
            {
                StartTransactionPerClient(mongoContexts);
                await next?.Invoke(context);
                await CommitTransaction(mongoContexts, successContexts);
            }
            catch
            {
                await RollbackTransaction(mongoContexts, successContexts);
                throw;
            }
            finally
            {
                ReleaseTransactionPerClient(mongoContexts);
            }
        }

        private async Task CommitTransaction(List<MongoContext> allContexts, List<MongoContext> successContexts)
        {
            foreach (var context in allContexts)
            {
                await context.Transaction.CommitTransactionAsync();
                successContexts.Add(context);
            }
        }
        private async Task RollbackTransaction(List<MongoContext> allContexts, List<MongoContext> successContexts)
        {
            foreach (var context in allContexts.Except(successContexts))
            {

                    await context.Transaction.AbortTransactionAsync();
              
                
               
            }
        }

        private void StartTransactionPerClient(IEnumerable<MongoContext> mongoContexts)
        {
            foreach (var context in mongoContexts)
            {
                context.Transaction = context.Client.StartSession();
                context.Transaction.StartTransaction(new TransactionOptions(
        readPreference: ReadPreference.Primary,
        readConcern: ReadConcern.Local,
        writeConcern: WriteConcern.WMajority));
            }
        }
        private void ReleaseTransactionPerClient(IEnumerable<MongoContext> mongoContexts)
        {
            foreach (var context in mongoContexts)
            {
                context.Transaction = null;
            }
        }
    }
}
