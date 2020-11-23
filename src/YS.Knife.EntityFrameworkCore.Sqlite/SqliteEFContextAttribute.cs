using System;

namespace Microsoft.EntityFrameworkCore
{
    public class SqliteEFContextAttribute : EFContextAttribute
    {
        public SqliteEFContextAttribute(params Type[] interceptorTypes) : base(interceptorTypes)
        {

        }
        public SqliteEFContextAttribute(string connectionStringKey, params Type[] interceptorTypes) : base(connectionStringKey, interceptorTypes)
        {
        }

        public override void BuildOptions(DbContextOptionsBuilder builder, string connectionString)
        {
            builder.UseSqlite(connectionString, (op) =>
            {

            });
        }
    }
}
