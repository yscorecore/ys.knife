namespace Microsoft.EntityFrameworkCore
{
    public class PostgreSqlDbContextClassAttribute : DbContextClassAttribute
    {
        public PostgreSqlDbContextClassAttribute(string connectionStringKey) : base(connectionStringKey)
        {

        }

        public override string DbType => "postgresql";

        public override void BuildOptions(DbContextOptionsBuilder builder, string connectionString)
        {
            builder.UseNpgsql(connectionString, (op) =>
            {
                op.EnableRetryOnFailure();
            });
        }
    }
}
