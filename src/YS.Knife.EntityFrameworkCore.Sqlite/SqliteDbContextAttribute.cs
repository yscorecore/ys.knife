namespace Microsoft.EntityFrameworkCore
{
    public class SqliteDbContextAttribute : DbContextAttribute
    {
        public SqliteDbContextAttribute(string connectionStringKey) : base(connectionStringKey)
        {
        }

        public override string DbType => "sqlite";

        public override void BuildOptions(DbContextOptionsBuilder builder, string connectionString)
        {
            builder.UseSqlite(connectionString, (op) =>
            {

            });
        }
    }
}
