namespace Microsoft.EntityFrameworkCore
{
    public class SqliteDbContextClassAttribute: DbContextClassAttribute
    {
        public SqliteDbContextClassAttribute(string connectionStringKey):base(connectionStringKey)
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
