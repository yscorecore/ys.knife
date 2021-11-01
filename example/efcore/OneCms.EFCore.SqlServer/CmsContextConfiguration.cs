using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using YS.Knife;

namespace OneCms.EFCore.SqlServer
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
            
            builder.UseSqlServer(configuration.GetConnectionString("cms") ?? "Server=localhost;Database=cms;User Id=sa;Password=Password@123;");
        }
    }
}
