using System;

namespace Microsoft.EntityFrameworkCore
{
    public class OracleEFContextAttribute : EFContextAttribute
    {
        public OracleEFContextAttribute(params Type[] interceptorTypes) : base(interceptorTypes)
        {

        }
        public OracleEFContextAttribute(string connectionStringKey, params Type[] interceptorTypes) : base(connectionStringKey, interceptorTypes)
        {

        }
        public override void BuildOptions(DbContextOptionsBuilder builder, string connectionString)
        {
            throw new NotImplementedException();
        }
    }
}
