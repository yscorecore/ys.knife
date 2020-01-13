using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.EntityFrameworkCore
{
    public class PostgreSqlDbContextLoader : IServiceLoader
    {
        public void LoadServices(IServiceCollection services, IConfiguration configuration)
        {
            foreach (var contextType in AppDomain.CurrentDomain.FindInstanceTypesByAttributeAndBaseType<PostgreSqlDbContextClassAttribute, DbContext>())
            {
                var attribute = contextType.GetCustomAttributes(typeof(PostgreSqlDbContextClassAttribute), false)[0] as PostgreSqlDbContextClassAttribute;
                var proxy = CreateRegisterProxy(contextType, attribute);
                proxy.AddDbContext(services, configuration, attribute);
            }
        }
        private IDbContextGenericProxy CreateRegisterProxy(Type contextType, PostgreSqlDbContextClassAttribute attribute)
        {
            var proxyType = attribute.InjectType != null
                    ? typeof(DbContextGenericProxy<,>).MakeGenericType(contextType, attribute.InjectType)
                    : typeof(DbContextGenericProxy<>).MakeGenericType(contextType);
            return Activator.CreateInstance(proxyType) as IDbContextGenericProxy;
        }

        #region InnerClass
        private interface IDbContextGenericProxy
        {
            void AddDbContext(IServiceCollection services, IConfiguration configuration, PostgreSqlDbContextClassAttribute attribute);
        }

        private class DbContextGenericProxy<ImplType> : IDbContextGenericProxy
             where ImplType : DbContext
        {
            public void AddDbContext(IServiceCollection services, IConfiguration configuration, PostgreSqlDbContextClassAttribute attribute)
            {
                string connectionStringKey = string.IsNullOrEmpty(attribute.ConnectionStringKey) ? typeof(ImplType).Name : attribute.ConnectionStringKey;
                services.AddDbContextPool<ImplType>((build) =>
                {
                    build.UseNpgsql(configuration.GetConnectionString(connectionStringKey), (op) =>
                    {
                        op.EnableRetryOnFailure();
                    });
                });
            }
        }
        private class DbContextGenericProxy<ImplType, InjectType> : IDbContextGenericProxy
               where InjectType : class
              where ImplType : DbContext, InjectType
        {
            public void AddDbContext(IServiceCollection services, IConfiguration configuration, PostgreSqlDbContextClassAttribute attribute)
            {
                string connectionStringKey = string.IsNullOrEmpty(attribute.ConnectionStringKey) ? typeof(ImplType).Name : attribute.ConnectionStringKey;
                services.AddDbContextPool<InjectType, ImplType>((build) =>
                 {
                     build.UseNpgsql(configuration.GetConnectionString(connectionStringKey), (op) =>
                     {
                         op.EnableRetryOnFailure();
                     });
                 });
            }
        }
        #endregion

    }
}
