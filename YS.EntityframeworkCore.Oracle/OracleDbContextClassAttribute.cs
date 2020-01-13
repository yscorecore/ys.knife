using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.EntityFrameworkCore
{
    public class OracleDbContextClassAttribute:Attribute
    {
        public OracleDbContextClassAttribute(string connectionStringKey)
        {
            this.ConnectionStringKey = connectionStringKey;
        }
       
        public Type InjectType { get; set; }

        public string ConnectionStringKey { get; set; }
    }
}
