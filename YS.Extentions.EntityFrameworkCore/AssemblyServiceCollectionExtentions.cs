using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Data;
using System.Data.Entity;
using Microsoft.EntityFrameworkCore;
using YS.Extentions.EntityFrameworkCore;
using System.Data.Store;

namespace System
{
    public static class AssemblyServiceCollectionExtentions
    {

        public const string DBContextConfigsKey = "DBContextConfigs";
        private static IConfigurationSection GetDbContextConfig(this IConfiguration configuration, string name)
        {
            IConfigurationSection section = configuration.GetSection(DBContextConfigsKey);
            if (section == null)
            {
                return null;
            }
            return section.GetSection(name);
        }
        public static Assembly AppendDbContexts(this Assembly assembly, IServiceCollection services, IConfiguration configuration)
        {
            var dbContextTypes = from p in assembly.GetTypes()
                                 where Attribute.IsDefined(p, typeof(DbContextClassAttribute))
                                       && !p.IsAbstract
                                 select p;
            foreach (var contextType in dbContextTypes)
            {
                var dbContextAttr = Attribute.GetCustomAttribute(contextType, typeof(DbContextClassAttribute)) as DbContextClassAttribute;

                var connectionKey = string.IsNullOrEmpty(dbContextAttr.ConnectStringKey) ? contextType.FullName : dbContextAttr.ConnectStringKey;

                var configKey = string.IsNullOrEmpty(dbContextAttr.ConfigKey) ? contextType.FullName : dbContextAttr.ConfigKey;

                var configSection = configuration.GetDbContextConfig(configKey);

                var config = configSection == null ? new DBContextConfig() : configSection.Get<DBContextConfig>() ?? new DBContextConfig();

                var connectionString = configuration.GetConnectionString(connectionKey);

                var contextInitInfo = new DBContextInitInfo()
                {
                    ConfigNode = configSection,
                    DbType = config.DbType,
                    AssemblyName = contextType.Assembly.GetName().Name,
                    ConnectionString = connectionString,
                    ContextFullName = contextType.FullName,
                    ContextName = contextType.Name,
                    ContextType = contextType,
                    MigrationAssemblyTemplate = config.MigrationAssemblyTemplate
                };
                AddDbContextInternal(services, contextInitInfo);
                if (config.RegisteEntityStore)
                {
                    AddEntityStoresInternal(services, contextType, config);
                }
            }

            return assembly;
        }

        private static void AddDbContextInternal(IServiceCollection services, DBContextInitInfo initContext)
        {
            var instance = Activator.CreateInstance(typeof(DbContextGenericProxy<>).MakeGenericType(initContext.ContextType)) as IDbContextGenericProxy;
            instance.AddDbContext(services, initContext);
        }
        private static void AddEntityStoresInternal(IServiceCollection services, Type contextType, DBContextConfig config)
        {
            var entityTypes = from p in contextType.GetProperties()
                             let pType=p.PropertyType
                             where pType.IsGenericType && pType.GetGenericTypeDefinition() == typeof(DbSet<>)
                             select pType.GetGenericArguments().First();
            foreach (var entityType in entityTypes)
            {
                var storeType = typeof(IEntityStore<>).MakeGenericType(entityType);
                var implType = typeof(EFEntityStore<,>).MakeGenericType(entityType, contextType);
                services.AddScoped(storeType, implType);
            }
        }


        #region InnerClass
        private interface IDbContextGenericProxy
        {
            void AddDbContext(IServiceCollection services, DBContextInitInfo initContext);
        }

        private class DbContextGenericProxy<T> : IDbContextGenericProxy
             where T : DbContext
        {
            public void AddDbContext(IServiceCollection services, DBContextInitInfo initContext)
            {
                var dbtype = initContext.DbType;
                var initHandler = DbContextOptionsBuilderHandlers.GetInitHandler(initContext.DbType);

                if (initHandler == null)
                {
                    throw new Exception($"找不到名称为{dbtype}的初始化程序，请调用{nameof(DbContextOptionsBuilderHandlers)}中的方法注册对应的Handler。");
                }

                services.AddDbContext<T>((option) =>
                 {
                     initHandler(initContext,option);
                 });
            }


        }
        #endregion
    }
}
