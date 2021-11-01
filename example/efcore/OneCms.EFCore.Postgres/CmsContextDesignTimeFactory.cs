using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OneCms.EFCore.Postgres
{
    public class CmsContextDesignTimeFactory : DesignTimeDbContextFactoryBase<CmsContext>
    {
    }
}
