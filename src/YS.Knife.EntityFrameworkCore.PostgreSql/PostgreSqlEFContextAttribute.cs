using System;

namespace Microsoft.EntityFrameworkCore
{
    public class PostgreSqlEFContextAttribute : EFContextAttribute
    {
        public PostgreSqlEFContextAttribute(params Type[] interceptorTypes) : base(interceptorTypes)
        {

        }
        public PostgreSqlEFContextAttribute(string connectionStringKey, params Type[] interceptorTypes) : base(connectionStringKey, interceptorTypes)
        {

        }
        public override void BuildOptions(DbContextOptionsBuilder builder, string connectionString)
        {
            builder.UseNpgsql(connectionString, (op) =>
            {
                op.UseQuerySplittingBehavior(QuerySplittingBehavior);
                op.EnableRetryOnFailure();
            });
        }
    }
}
