using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public class DbContextOptionsBuilderHandlers
    {
        private static Dictionary<string, Action<DBContextInitInfo, DbContextOptionsBuilder>> Handlers =
            new Dictionary<string, Action<DBContextInitInfo, DbContextOptionsBuilder>>(StringComparer.CurrentCultureIgnoreCase);

        public static Action<DBContextInitInfo, DbContextOptionsBuilder> GetInitHandler(string dbtype)
        {
            return Handlers[dbtype];
        }
        public static void Register(string name, Action<DBContextInitInfo, DbContextOptionsBuilder> initHandler)
        {
            Handlers[name] = initHandler;
        }
    }
}
