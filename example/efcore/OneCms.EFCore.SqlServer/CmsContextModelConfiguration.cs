﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YS.Knife;

namespace OneCms.EFCore.SqlServer
{
    [Service]
    public class CmsContextModelConfiguration : DbContextModelConfigration<CmsContext>
    {

    }
}
