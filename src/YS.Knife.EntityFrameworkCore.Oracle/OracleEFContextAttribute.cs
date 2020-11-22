using System;

namespace Microsoft.EntityFrameworkCore
{
    public class OracleEFContextAttribute : EFContextAttribute
    {
        public OracleEFContextAttribute()
        {

        }
        public OracleEFContextAttribute(string connectionStringKey) : base(connectionStringKey)
        {

        }
        public override void BuildOptions(DbContextOptionsBuilder builder, string connectionString)
        {
            throw new NotImplementedException();
        }
    }
}
