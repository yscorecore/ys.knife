namespace Microsoft.EntityFrameworkCore
{
    public class SqliteEFContextAttribute : EFContextAttribute
    {
        public SqliteEFContextAttribute()
        {

        }
        public SqliteEFContextAttribute(string connectionStringKey) : base(connectionStringKey)
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
