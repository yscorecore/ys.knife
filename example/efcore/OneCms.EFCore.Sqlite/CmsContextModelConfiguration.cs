using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YS.Knife;
using YS.Knife.EntityFrameworkCore;

namespace OneCms.EFCore.Sqlite
{
    [Service]
    public class CmsContextModelConfiguration : DbContextModelConfigration<CmsContext>
    {
        public override void ConfigModels(ModelBuilder modelBuilder)
        {
            base.ConfigModels(modelBuilder);
        }
    }
}
