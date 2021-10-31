using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace YS.Knife.EntityFrameworkCore.Interceptors
{
    public class TenantCommandInterceptor : DbCommandInterceptor
    {
        ITenantProvider tenantProvider;
        public TenantCommandInterceptor(ITenantProvider tenantProvider)
        {
            this.tenantProvider = tenantProvider;
        }
        public override InterceptionResult<DbDataReader> ReaderExecuting(
            DbCommand command, CommandEventData eventData,
            InterceptionResult<DbDataReader> result)
        {
            ModifyCommand(command);
            return base.ReaderExecuting(command, eventData, result);
        }
        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command, CommandEventData eventData,
            InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            ModifyCommand(command);
            return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
        }
        private void ModifyCommand(DbCommand command)
        {
            if (command.CommandText.StartsWith("UPDATE \"BlogPosts\"") ||
                command.CommandText.StartsWith("DELETE FROM \"BlogPosts\""))
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@__TenantId__";
                parameter.Value = tenantProvider.GetTenantId();
                command.Parameters.Add(parameter);
                command.CommandText = command.CommandText.Replace("WHERE",
                    $"WHERE (\"TenantId\" = {parameter.ParameterName}) AND ");
            }
        }
    }
}
