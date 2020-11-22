using System;

namespace Microsoft.EntityFrameworkCore
{
    public class OracleDbContextAttribute : EFContextAttribute
    {
        public OracleDbContextAttribute()
        {

        }
        public OracleDbContextAttribute(string connectionStringKey) : base(connectionStringKey)
        {

        }
        public override void BuildOptions(DbContextOptionsBuilder builder, string connectionString)
        {
            throw new NotImplementedException();
        }
    }
}
