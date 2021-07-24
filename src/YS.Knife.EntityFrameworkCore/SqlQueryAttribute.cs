using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using Microsoft.EntityFrameworkCore;
using YS.Knife.Aop;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace YS.Knife.EntityFrameworkCore
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SqlQueryAttribute : BaseAopAttribute
    {
        private static MethodInfo SetMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set),Type.EmptyTypes);

        private static MethodInfo FromRowSqlMethod = typeof(RelationalQueryableExtensions).GetMethod(nameof(RelationalQueryableExtensions.FromSqlRaw));
        public SqlQueryAttribute(string sql)
        {
            this.Sql = sql;
        }
        public string Sql { get; set; }

        public override Task Invoke(AspectContext context, AspectDelegate next)
        {
            var dbContext = GetDbContext(context);
            var entityType = GetEntityType(context);
            var dbSet = GetDbSet(dbContext, entityType);
            context.ReturnValue = ExecuteSql(entityType, dbSet, context.Parameters);
            return context.Break();
        }
        private DbContext GetDbContext(AspectContext context)
        {
            var definedType = context.ServiceMethod.DeclaringType;
            var rowSql = definedType.GetCustomAttribute<RowSqlAttribute>();
            if (rowSql == null)
            {
                throw new InvalidOperationException($"missing '{nameof(RowSqlAttribute)}' defined in type '{definedType}'.");
            }
            return context.ServiceProvider.GetRequiredService(rowSql.DbContextType) as DbContext;
        }
        private Type GetEntityType(AspectContext context)
        {
            var retrnType = context.ServiceMethod.ReturnType;
            return retrnType.GetEnumerableItemType() ?? retrnType;
        }
        private object GetDbSet(DbContext dbContext, Type entityType)
        {
            var method = SetMethod.MakeGenericMethod(entityType);
            return method.Invoke(dbContext,Array.Empty<object>());
        }
        private object ExecuteSql(Type entityType,object dbSet, object[] args)
        {
            var method = FromRowSqlMethod.MakeGenericMethod(entityType);
            return method.Invoke(null, new object[] { dbSet, this.Sql, args });
        }
    }
}
