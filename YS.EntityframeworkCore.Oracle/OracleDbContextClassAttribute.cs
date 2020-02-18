using System;

namespace Microsoft.EntityFrameworkCore
{
    public class OracleDbContextClassAttribute: DbContextClassAttribute
    {
        public OracleDbContextClassAttribute(string connectionStringKey):base(connectionStringKey)
        {
          
        }

        public override string DbType => "oracle";

        public override void BuildOptions(DbContextOptionsBuilder builder, string connectionString)
        {
            throw new NotImplementedException();
        }
    }
}
