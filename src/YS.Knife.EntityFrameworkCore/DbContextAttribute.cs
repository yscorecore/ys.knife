using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YS.Knife;

namespace Microsoft.EntityFrameworkCore
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public abstract class DbContextAttribute : KnifeAttribute
    {
        public DbContextAttribute(string connectionStringKey) : base(typeof(DbContext))
        {
            this.ConnectionStringKey = connectionStringKey;
        }

        public string ConnectionStringKey { get; set; }

        public abstract void BuildOptions(DbContextOptionsBuilder builder, string connectionString);

        public override void RegisterService(IServiceCollection services, IRegisteContext context, Type declareType)
        {
            _ = declareType ?? throw new ArgumentNullException(nameof(declareType));
            _ = context ?? throw new ArgumentNullException(nameof(context));


            string connectionStringKey = string.IsNullOrEmpty(this.ConnectionStringKey)
                ? declareType.Name
                : this.ConnectionStringKey;
            string connectionString = context.Configuration.GetConnectionString(connectionStringKey);
            if (String.IsNullOrEmpty(connectionString))
            {
                throw new ApplicationException($"Can not find connection string by key \"{connectionStringKey}\".");
            }
            var injectType = declareType;
            while (injectType != typeof(DbContext))
            {
                var method = typeof(DbContextAttribute)
                    .GetMethod(nameof(AddDbContext2), BindingFlags.Instance | BindingFlags.NonPublic)
                    ?.MakeGenericMethod(injectType, declareType);
                method.Invoke(this, new object[] { services, connectionString });
                injectType = injectType.BaseType;
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
    }
}
