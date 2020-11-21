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

                foreach (var tran in group.Transactions)
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

        private class DefaultTransactionContext : ITransactionContext
        {

            public List<ITransaction> Transactions => new List<ITransaction>();

            public void Dispose()
            {

            }
        }
    }
    
}
