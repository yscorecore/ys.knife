namespace Microsoft.EntityFrameworkCore
{
    public class SqlServerDbContextAttribute : DbContextAttribute
    {
        public SqlServerDbContextAttribute()
        {

        }
        public SqlServerDbContextAttribute(string connectionStringKey) : base(connectionStringKey)
        {
            this.ConnectionStringKey = connectionStringKey;
        }
        public override void BuildOptions(DbContextOptionsBuilder builder, string connectionString)
        {
            builder.UseSqlServer(connectionString, (op) =>
            {
                op.EnableRetryOnFailure();
            });
        }
    }
}
