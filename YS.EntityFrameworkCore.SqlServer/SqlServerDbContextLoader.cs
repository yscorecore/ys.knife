using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.EntityFrameworkCore
{
    public class SqlServerDbContextLoader : IServiceLoader
    {
        public void LoadServices(IServiceCollection services, IConfiguration configuration)
        {
            foreach (var contextType in AppDomain.CurrentDomain.FindInstanceTypesByAttributeAndBaseType<SqlServerDbContextClassAttribute, DbContext>())
            {
                var attribute = contextType.GetCustomAttributes(typeof(SqlServerDbContextClassAttribute), false)[0] as SqlServerDbContextClassAttribute;
                var proxy = CreateRegisterProxy(contextType, attribute);
                proxy.AddDbContext(services, configuration, attribute);
            }
        }
        private IDbContextGenericProxy CreateRegisterProxy(Type contextType, SqlServerDbContextClassAttribute attribute)
        {
            var proxyType = attribute.InjectType != null
                    ? typeof(DbContextGenericProxy<,>).MakeGenericType(contextType, attribute.InjectType)
                    : typeof(DbContextGenericProxy<>).MakeGenericType(contextType);
            return Activator.CreateInstance(proxyType) as IDbContextGenericProxy;
        }

        #region InnerClass
        private interface IDbContextGenericProxy
        {
            void AddDbContext(IServiceCollection services, IConfiguration configuration, SqlServerDbContextClassAttribute attribute);
        }

        private class DbContextGenericProxy<ImplType> : IDbContextGenericProxy
             where ImplType : DbContext
        {
            public void AddDbContext(IServiceCollection services, IConfiguration configuration, SqlServerDbContextClassAttribute attribute)
            {
                string connectionStringKey = string.IsNullOrEmpty(attribute.ConnectionStringKey) ? typeof(ImplType).Name : attribute.ConnectionStringKey;
                services.AddDbContext<ImplType>((build) =>
                {
                    build.UseSqlServer(configuration.GetConnectionString(connectionStringKey));
                });
            }
        }
        private class DbContextGenericProxy<ImplType, InjectType> : IDbContextGenericProxy
               where InjectType : class
              where ImplType : DbContext, InjectType
        {
            public void AddDbContext(IServiceCollection services, IConfiguration configuration, SqlServerDbContextClassAttribute attribute)
            {
                string connectionStringKey = string.IsNullOrEmpty(attribute.ConnectionStringKey) ? typeof(ImplType).Name : attribute.ConnectionStringKey;
                services.AddDbContextPool<InjectType, ImplType>((build) =>
                 {
                     build.UseSqlServer(configuration.GetConnectionString(connectionStringKey), (op) =>
                     {
                         op.EnableRetryOnFailure();
                     });
                 }, 64);
            }
        }
        #endregion

    }
}
