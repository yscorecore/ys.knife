using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using YS.Knife.EntityFrameworkCore;

namespace OneCms.EFCore
{

    [KnifeEFContext]
    public class CmsContext : KnifeDbContext
    {
        public CmsContext(DbContextOptions<CmsContext> dbContextOptions, DbContextModelConfigration<CmsContext> dbModelConfigration) :
            base(dbContextOptions, dbModelConfigration)
        {

        }

        public virtual DbSet<Post> Posts { get; set; }

        public virtual DbSet<Topic> Topics { get; set; }

    }


}
