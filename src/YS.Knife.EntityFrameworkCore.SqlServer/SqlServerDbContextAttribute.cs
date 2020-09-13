namespace Microsoft.EntityFrameworkCore
{
    public class SqlServerDbContextAttribute : DbContextAttribute
    {
        public SqlServerDbContextAttribute(string connectionStringKey) : base(connectionStringKey)
        {
            this.ConnectionStringKey = connectionStringKey;
        }

        public override string DbType => "mssql";

        public override void BuildOptions(DbContextOptionsBuilder builder, string connectionString)
        {
            builder.UseSqlServer(connectionString, (op) =>
            {
                op.EnableRetryOnFailure();
            });
        }
    }
}
