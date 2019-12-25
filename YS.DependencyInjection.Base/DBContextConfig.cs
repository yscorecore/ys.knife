using System;
using System.Collections.Generic;
using System.Text;

namespace System
{

    public class DBContextConfig
    {
        public string DbType { get; set; } = "SqlServer";
        public string MigrationAssemblyTemplate { get; set; } = "Host.Database.{DbType}";
        public bool AutoMigration = true;
        public bool RegisteEntityStore { get; set; } = true;
    }
}
