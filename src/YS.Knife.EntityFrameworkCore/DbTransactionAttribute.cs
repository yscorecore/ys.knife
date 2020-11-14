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
    public class DbTransactionAttribute : Aop.BaseAopAttribute
    {
        private Type[] commandTypes;

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            using (var group = new DefaultTransactionGroup())
            {

                var instances = commandTypes.Select(p => context.ServiceProvider.GetService(p)).OfType<ITransactionCommand>().ToList();


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
    public interface IDBTransactionGroup:IDisposable
    {
        Dictionary<string, ITransaction> Transactions { get; set; }
    }
    public interface ITransaction
    {
        void Begin();
        void Commit();
        void Rollback();
    }
    public class DefaultTransactionGroup:IDBTransactionGroup
    { 
       public Dictionary<string, ITransaction> Transactions { get; set; }

        public void Dispose()
        {
            
        }
    }
    

    public interface ITransactionCommand
    {
        void UseTransaction(IDBTransactionGroup transactionGroup);
    }
}
