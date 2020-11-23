using System;
using YS.Knife;

namespace Microsoft.EntityFrameworkCore
{
    public class MySqlEFContextAttribute : EFContextAttribute
    {
        public MySqlEFContextAttribute(params Type[] interceptorTypes) : base(interceptorTypes)
        {

        }
        public MySqlEFContextAttribute(string connectionStringKey, params Type[] interceptorTypes) : base(connectionStringKey, interceptorTypes)
        {
        }
        public override void BuildOptions(DbContextOptionsBuilder builder, string connectionString)
        {
            builder.UseMySql(connectionString, (op) =>
            {
                op.EnableRetryOnFailure();
            });
        }
    }


}
