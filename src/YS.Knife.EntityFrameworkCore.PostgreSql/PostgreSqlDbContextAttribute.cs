namespace Microsoft.EntityFrameworkCore
{
    public class PostgreSqlDbContextAttribute : DbContextAttribute
    {
        public PostgreSqlDbContextAttribute(string connectionStringKey) : base(connectionStringKey)
        {

        }
        public override void BuildOptions(DbContextOptionsBuilder builder, string connectionString)
        {
            builder.UseNpgsql(connectionString, (op) =>
            {
                op.EnableRetryOnFailure();
            });
        }
    }
}
