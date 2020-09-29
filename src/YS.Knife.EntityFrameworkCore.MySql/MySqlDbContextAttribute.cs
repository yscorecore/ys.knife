using YS.Knife;

namespace Microsoft.EntityFrameworkCore
{
    public class MySqlDbContextAttribute : DbContextAttribute
    {
        public MySqlDbContextAttribute(string connectionStringKey) : base(connectionStringKey)
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
