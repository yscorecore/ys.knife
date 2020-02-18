using Knife;
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
            var options = configuration.GetConfigOrNew<AppOptions>();
            foreach (var contextType in AppDomain.CurrentDomain.FindInstanceTypesByAttributeAndBaseType<T, DbContext>())
            {
                var attribute = contextType.GetCustomAttributes(typeof(T), false)[0] as T;
                // filter dbtype
                if (CanRegister(options, attribute))
                {
                    var proxy = CreateRegisterProxy(contextType, attribute);
                    proxy.AddDbContext(services, configuration, attribute);
                }
            }
        }
        private bool CanRegister(AppOptions appOptions,T dbContextAttribute)
        {
            return string.Equals(appOptions.DbType, "[all]", StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(dbContextAttribute.DbType, appOptions.DbType, StringComparison.InvariantCultureIgnoreCase);
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
            void AddDbContext(IServiceCollection services, IConfiguration configuration, T attribute);
        }

        private class DbContextGenericProxy<ImplType> : IDbContextGenericProxy
             where ImplType : DbContext
        {
            public void AddDbContext(IServiceCollection services, IConfiguration configuration, T attribute)
            {
                string connectionStringKey = string.IsNullOrEmpty(attribute.ConnectionStringKey) ? typeof(ImplType).Name : attribute.ConnectionStringKey;
                services.AddDbContextPool<ImplType>((build) =>
                {
                    attribute.BuildOptions(build, configuration.GetConnectionString(connectionStringKey));
                });
            }
        }
        private class DbContextGenericProxy<ImplType, InjectType> : IDbContextGenericProxy
               where InjectType : class
              where ImplType : DbContext, InjectType
        {
            public void AddDbContext(IServiceCollection services, IConfiguration configuration, T attribute)
            {
                string connectionStringKey = string.IsNullOrEmpty(attribute.ConnectionStringKey) ? typeof(ImplType).Name : attribute.ConnectionStringKey;
                services.AddDbContextPool<InjectType, ImplType>((build) =>
                {
                    attribute.BuildOptions(build, configuration.GetConnectionString(connectionStringKey));
                });
            }
        }
        #endregion
    }
}
