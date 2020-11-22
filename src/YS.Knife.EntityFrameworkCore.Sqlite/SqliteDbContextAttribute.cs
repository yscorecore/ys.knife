namespace Microsoft.EntityFrameworkCore
{
    public class SqliteDbContextAttribute : EFContextAttribute
    {
        public SqliteDbContextAttribute()
        {

        }
        public SqliteDbContextAttribute(string connectionStringKey) : base(connectionStringKey)
        {
        }

        public override void BuildOptions(DbContextOptionsBuilder builder, string connectionString)
        {
            builder.UseSqlite(connectionString, (op) =>
            {

            });
        }
    }
}
