using YS.Knife;

namespace Microsoft.EntityFrameworkCore
{
    public class MySqlEFContextAttribute : EFContextAttribute
    {
        public MySqlEFContextAttribute()
        {

        }
        public MySqlEFContextAttribute(string connectionStringKey) : base(connectionStringKey)
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
