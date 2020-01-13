using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.EntityFrameworkCore
{
    public class SqliteDbContextClassAttribute:Attribute
    {
        public SqliteDbContextClassAttribute(string connectionStringKey)
        {
            this.ConnectionStringKey = connectionStringKey;
        }
       
        public Type InjectType { get; set; }

        public string ConnectionStringKey { get; set; }
    }
}
