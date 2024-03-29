﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using YS.Knife;
using YS.Knife.EntityFrameworkCore;

namespace OneCms.EFCore.Sqlite
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
            builder.UseSqlite(configuration.GetConnectionString("cms") ?? "Data Source=cms.db");
        }
    }
}
