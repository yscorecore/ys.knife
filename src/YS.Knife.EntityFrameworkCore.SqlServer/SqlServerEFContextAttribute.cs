namespace Microsoft.EntityFrameworkCore
{
    public class SqlServerEFContextAttribute : EFContextAttribute
    {
        public SqlServerEFContextAttribute()
        {

        }
        public SqlServerEFContextAttribute(string connectionStringKey) : base(connectionStringKey)
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
