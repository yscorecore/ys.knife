using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq;

namespace YS.Knife.EntityFrameworkCore
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TransactionAttribute : Aop.BaseAopAttribute
    {
        private Type[] commandTypes;

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            using (var group = new DefaultTransactionContext())
            {

                var instances = commandTypes.Select(p => context.ServiceProvider.GetService(p)).OfType<ITransactionStep>().ToList();


                foreach (var instance in instances)
                {
                    instance.UseTransaction(group);
                }
                await next.Invoke(context);

                foreach (var tran in group.Transactions.Values)
                {
                    try
                    {
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }

        }
    }
    public interface ITransactionContext:IDisposable
    {
        Dictionary<string, ITransaction> Transactions { get;  }
    }
    public interface ITransaction
    {
        void Commit();
        void Rollback();
    }
    public class DefaultTransactionContext:ITransactionContext
    {
        public Dictionary<string, ITransaction> Transactions { get; private set; } = new Dictionary<string, ITransaction>();

        public void Dispose()
        {
            
        }
    }
    

    public interface ITransactionStep
    {
        void UseTransaction(ITransactionContext transactionContext);
    }
}
