using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.EntityFrameworkCore
{
    public abstract class DbContextClassAttribute : Attribute
    {
        public DbContextClassAttribute(string connectionStringKey)
        {
            this.ConnectionStringKey = connectionStringKey;
        }

        public Type InjectType { get; set; }

        public string ConnectionStringKey { get; set; }

        public abstract string DbType { get; }

        public abstract void BuildOptions(DbContextOptionsBuilder builder, string connectionString);
    }
}
