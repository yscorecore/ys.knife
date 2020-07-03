using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YS.Knife;

namespace Microsoft.EntityFrameworkCore
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public abstract class DbContextClassAttribute : KnifeAttribute
    {
        public DbContextClassAttribute(string connectionStringKey) : base(typeof(DbContext))
        {
            this.ConnectionStringKey = connectionStringKey;
        }

        public string ConnectionStringKey { get; set; }

        public abstract string DbType { get; }

        public abstract void BuildOptions(DbContextOptionsBuilder builder, string connectionString);

        public override void RegisteService(IServiceCollection services, IRegisteContext context, Type declareType)
        {
            _ = declareType ?? throw new ArgumentNullException(nameof(declareType));
            _ = context ?? throw new ArgumentNullException(nameof(context));


            string connectionStringKey = string.IsNullOrEmpty(this.ConnectionStringKey) ? declareType.Name : this.ConnectionStringKey;

            if (CanRegister(context.Configuration, connectionStringKey, out string connectionString))
            {
                var injectType = declareType;
                while (injectType != typeof(DbContext))
                {
                    var method = typeof(DbContextClassAttribute).GetMethod(nameof(AddDbContext2), BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(injectType, declareType);
                    method.Invoke(this, new object[] { services, connectionString });
                    injectType = injectType.BaseType;
                }
            }
        }
        private void AddDbContext2<InjectType, ImplType>(IServiceCollection services, string connectionString)
            where InjectType : class
               where ImplType : DbContext, InjectType
        {
            services.AddDbContextPool<InjectType, ImplType>((build) =>
            {
                this.BuildOptions(build, connectionString);
            });
        }
        private bool CanRegister(IConfiguration configuration, string connectionKey, out string connectionString)
        {
            connectionString = string.Empty;
            // 存在已经匹配的连接字符串时才进行注册
            var connectionInfo = configuration.GetConnectionInfo(connectionKey);
            if (connectionInfo != null) connectionString = connectionInfo.Value;
            return connectionInfo != null && string.Equals(connectionInfo.DBType, this.DbType, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
