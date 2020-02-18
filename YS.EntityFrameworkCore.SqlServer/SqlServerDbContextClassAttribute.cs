namespace Microsoft.EntityFrameworkCore
{
    public class SqlServerDbContextClassAttribute: DbContextClassAttribute
    {
        public SqlServerDbContextClassAttribute(string connectionStringKey):base(connectionStringKey)
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
