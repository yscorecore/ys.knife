using System;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore
{
    public class SqlServerEFContextAttribute : EFContextAttribute
    {
        public SqlServerEFContextAttribute(params Type[] interceptorTypes) : base(interceptorTypes)
        {

        }
        public SqlServerEFContextAttribute(string connectionStringKey, params Type[] interceptorTypes) : base(connectionStringKey, interceptorTypes)
        {
            this.ConnectionStringKey = connectionStringKey;
        }
        public override void BuildOptions(DbContextOptionsBuilder builder, string connectionString)
        {
            builder.UseSqlServer(connectionString, (op) =>
            {
                op.UseQuerySplittingBehavior(QuerySplittingBehavior);
                op.EnableRetryOnFailure();
            });
        }
    }
}
