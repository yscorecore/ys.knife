using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YS.Knife;
using YS.Knife.EntityFrameworkCore;

namespace Microsoft.EntityFrameworkCore
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public abstract class DbContextAttribute : KnifeAttribute
    {
        public DbContextAttribute() : base(typeof(DbContext))
        {

        }
        public DbContextAttribute(string connectionStringKey) : base(typeof(DbContext))
        {
            this.ConnectionStringKey = connectionStringKey;
        }

        public string ConnectionStringKey { get; set; }

        public bool RegisteEntityStore { get; set; } = true;

        public bool RegisteAutoSubmitContext { get; set; } = true;

        public abstract void BuildOptions(DbContextOptionsBuilder builder, string connectionString);

        public override void RegisterService(IServiceCollection services, IRegisteContext context, Type declareType)
        {
            _ = declareType ?? throw new ArgumentNullException(nameof(declareType));
            _ = context ?? throw new ArgumentNullException(nameof(context));


            string connectionStringKey = string.IsNullOrEmpty(this.ConnectionStringKey)
                ? declareType.Name
                : this.ConnectionStringKey;
            string connectionString = context.Configuration.GetConnectionString(connectionStringKey);
            if (string.IsNullOrEmpty(connectionString))
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
            if (RegisteEntityStore)
            {
                AddEntityStoresInternal(services, declareType);
            }
            if (RegisteAutoSubmitContext)
            {
                services.AddScoped<ICommitEFChangesContext>(sp =>
                {
                    var dbcontext = sp.GetService(declareType) as DbContext;
                    return new AutoSubmitContext(dbcontext);
                });
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

        private static void AddEntityStoresInternal(IServiceCollection services, Type contextType)
        {
            var entityTypes = from p in contextType.GetProperties()
                              let pType = p.PropertyType
                              where pType.IsGenericType && pType.GetGenericTypeDefinition() == typeof(DbSet<>)
                              select pType.GetGenericArguments().First();
            foreach (var entityType in entityTypes)
            {
                var storeType = typeof(IEntityStore<>).MakeGenericType(entityType);
                var implType = typeof(EFEntityStore<,>).MakeGenericType(entityType, contextType);
                services.AddScoped(storeType, implType);
            }
        }

        private class AutoSubmitContext : ICommitEFChangesContext
        {
            public AutoSubmitContext(DbContext dbContext)
            {
                this.DbContext = dbContext;
            }

            public DbContext DbContext { get; }
        }
    }
}
