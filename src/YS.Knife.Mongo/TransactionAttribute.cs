using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using YS.Knife.Aop;
using MongoDB.Driver;
using YS.Knife.Data.Transactions;

namespace YS.Knife.Mongo
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class TransactionAttribute : BaseAopAttribute
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));
            _ = next ?? throw new ArgumentNullException(nameof(next));
            ITransactionManagement transactionManagement = GetCurrentTransactionManagement(context);
            if (transactionManagement == null)
            {
                var logger =
                    context.ServiceProvider.GetService(
                        typeof(ILogger<>).MakeGenericType(context.ImplementationMethod.DeclaringType)) as ILogger;
                logger.LogWarning($"Can not find transaction management in current type '{context.ImplementationMethod.DeclaringType}', {nameof(TransactionAttribute)} will be ignored.");
                await  next?.Invoke(context);
               return;
            }

            bool started = false;
            try
            {
                started = transactionManagement.StartTransaction();
                await next?.Invoke(context);
                if (started)
                {
                    transactionManagement.CommitTransaction();
                }
            }
            catch
            {
                if (started)
                {
                    transactionManagement.RollbackTransaction();
                }
                throw;
            }
            finally
            {
                if (started)
                {
                    transactionManagement.ResetTransaction();
                }
            }
        }

        private ITransactionManagement GetCurrentTransactionManagement(AspectContext context)
        {
            var transactionManagements = context.ImplementationMethod.DeclaringType.GetFields(BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic)
                .Select(p => p.GetValue(context.Implementation))
                .Where(p => p is ITransactionManagerProvider)
                .OfType<ITransactionManagerProvider>()
                .Select(p=>p.GetTransactionManagement())
                .Distinct()
                .ToList();
            if (transactionManagements.Count > 1)
            {
                throw new Exception($"Can't deduce transaction management, there are too many transaction management fields in '{context.ImplementationMethod.DeclaringType}' type.");
            }

            return transactionManagements.FirstOrDefault();
        }


    }




}
