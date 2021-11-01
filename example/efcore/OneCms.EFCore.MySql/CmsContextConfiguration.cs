using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using YS.Knife;

namespace OneCms.EFCore.MySql
{
    [Service]
    public class CmsContextConfiguration : DbContextConfigration<CmsContext>
    {
        private readonly IConfiguration configuration;

        public CmsContextConfiguration(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        protected override void OnConfigOptions(IServiceProvider sp, DbContextOptionsBuilder builder)
        {
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 26));
            builder.UseMySql(configuration.GetConnectionString("cms") ?? "server=localhost;user=root;password=password;database=cms", serverVersion);
        }
    }
}
