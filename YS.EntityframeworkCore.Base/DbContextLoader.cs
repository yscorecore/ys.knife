using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.EntityFrameworkCore
{
    public abstract class DbContextLoader<T> : IServiceLoader
         where T : DbContextClassAttribute

    {
        public void LoadServices(IServiceCollection services, IConfiguration configuration)
        {
            foreach (var contextType in AppDomain.CurrentDomain.FindInstanceTypesByAttributeAndBaseType<T, DbContext>())
            {
                var attribute = contextType.GetCustomAttributes(typeof(T), false)[0] as T;
                string connectionStringKey = string.IsNullOrEmpty(attribute.ConnectionStringKey) ? contextType.Name : attribute.ConnectionStringKey;
                // filter dbtype
                if (CanRegister(configuration, attribute.DbType, connectionStringKey, out string connectionString))
                {
                    var proxy = CreateRegisterProxy(contextType, attribute);
                    proxy.AddDbContext(services, attribute, connectionString);
                }
            }
        }
        private bool CanRegister(IConfiguration configuration, string dbType, string connectionKey, out string connectionString)
        {
            connectionString = string.Empty;
            var connectionInfo = configuration.GetConnectionInfo(connectionKey);
            if (connectionInfo != null) connectionString = connectionInfo.Value;
            return connectionInfo != null && string.Equals(connectionInfo.DBType, dbType, StringComparison.InvariantCultureIgnoreCase);
        }
        private IDbContextGenericProxy CreateRegisterProxy(Type contextType, T attribute)
        {
            var proxyType = attribute.InjectType != null
                    ? typeof(DbContextGenericProxy<,>).MakeGenericType(contextType, attribute.InjectType)
                    : typeof(DbContextGenericProxy<>).MakeGenericType(contextType);
            return Activator.CreateInstance(proxyType) as IDbContextGenericProxy;
        }

        #region InnerClass
        private interface IDbContextGenericProxy
        {
            void AddDbContext(IServiceCollection services, T attribute, string connectionString);
        }

        private class DbContextGenericProxy<ImplType> : IDbContextGenericProxy
             where ImplType : DbContext
        {
            public void AddDbContext(IServiceCollection services, T attribute, string connectionString)
            {
                services.AddDbContextPool<ImplType>((build) =>
                {
                    attribute.BuildOptions(build, connectionString);
                });
            }
        }
        private class DbContextGenericProxy<ImplType, InjectType> : IDbContextGenericProxy
               where InjectType : class
              where ImplType : DbContext, InjectType
        {
            public void AddDbContext(IServiceCollection services, T attribute, string connectionString)
            {
                services.AddDbContextPool<InjectType, ImplType>((build) =>
                {
                    attribute.BuildOptions(build, connectionString);
                });
            }
        }
        #endregion
    }
}
