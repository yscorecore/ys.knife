namespace Microsoft.EntityFrameworkCore
{
    public class PostgreSqlEFContextAttribute : EFContextAttribute
    {
        public PostgreSqlEFContextAttribute()
        {

        }
        public PostgreSqlEFContextAttribute(string connectionStringKey) : base(connectionStringKey)
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
