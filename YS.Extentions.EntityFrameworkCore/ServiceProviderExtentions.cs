using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Reflection;

namespace System
{
   public static  class ServiceProviderExtentions
    {
        public static void MigrateDbContextDatabaseAsync(this IServiceProvider serviceProvider, params string[] wildcardPatterns)
        {
            using (var serviceScope = serviceProvider.CreateScope())
            {
                //Assembly.GetEntryAssembly().LoadRefrenceAssembly(wildcardPatterns, (assembly) =>
                //{
                //    var dbContextTypes = from p in assembly.GetTypes()
                //                         where Attribute.IsDefined(p, typeof(DbContextClassAttribute))
                //                               && !p.IsAbstract
                //                         select p;
                //    foreach (var contextType in dbContextTypes)
                //    {
                //        var contextInstance = serviceScope.ServiceProvider.GetService(contextType) as DbContext;
                //        contextInstance.Database.Migrate();
                //    }

                //});


            }
        }

    }
}
