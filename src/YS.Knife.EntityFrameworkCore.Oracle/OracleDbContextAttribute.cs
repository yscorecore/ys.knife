using System;

namespace Microsoft.EntityFrameworkCore
{
    public class OracleDbContextAttribute : DbContextAttribute
    {
        public OracleDbContextAttribute(string connectionStringKey) : base(connectionStringKey)
        {

        }

        public override string DbType => "oracle";

        public override void BuildOptions(DbContextOptionsBuilder builder, string connectionString)
        {
            throw new NotImplementedException();
        }
    }
}
