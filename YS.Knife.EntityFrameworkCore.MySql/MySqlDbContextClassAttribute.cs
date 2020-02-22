namespace Microsoft.EntityFrameworkCore
{
    public class MySqlDbContextClassAttribute: DbContextClassAttribute
    {
        public MySqlDbContextClassAttribute(string connectionStringKey):base(connectionStringKey)
        {
        }

        public override string DbType => "mysql";

        public override void BuildOptions(DbContextOptionsBuilder builder, string connectionString)
        {
            builder.UseMySql(connectionString, (op) =>
            {
                op.EnableRetryOnFailure();
            });
        }
    }

   
}
