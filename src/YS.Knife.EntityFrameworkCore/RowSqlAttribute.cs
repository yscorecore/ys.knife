using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using YS.Knife.Aop;

namespace YS.Knife.EntityFrameworkCore
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class RowSqlAttribute : DynamicProxyAttribute
    {
        public RowSqlAttribute(Type dbContextType) : base(ServiceLifetime.Singleton)
        {
            _ = dbContextType ?? throw new ArgumentNullException(nameof(dbContextType));
            if (!typeof(DbContext).IsAssignableFrom(dbContextType))
            {
                throw new ArgumentException($"The type '{typeof(DbContext)}' can not assignable from '{dbContextType}'.", nameof(dbContextType));
            }
            this.DbContextType = dbContextType;
        }
        public Type DbContextType { get; set; }
    }
}
