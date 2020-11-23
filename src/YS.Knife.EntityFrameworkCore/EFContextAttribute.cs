using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YS.Knife;
using YS.Knife.EntityFrameworkCore;

namespace Microsoft.EntityFrameworkCore
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public abstract class EFContextAttribute : KnifeAttribute
    {
        public EFContextAttribute(params Type[] interceptorTypes) : base(typeof(DbContext))
        {
            this.InterceptorTypes = interceptorTypes;
            this.CheckInterceptorTypes(interceptorTypes);
        }
        public EFContextAttribute(string connectionStringKey, params Type[] interceptorTypes) : base(typeof(DbContext))
        {
            this.ConnectionStringKey = connectionStringKey;
            this.InterceptorTypes = interceptorTypes;
            this.CheckInterceptorTypes(interceptorTypes);
        }

        public string ConnectionStringKey { get; set; }

        public Type[] InterceptorTypes { get; }

        public bool RegisteEntityStore { get; set; } = true;

        public bool RegisteAutoSubmitContext { get; set; } = true;


        private void CheckInterceptorTypes(Type[] interceptorTypes)
        {
            if (interceptorTypes != null)
            {
                foreach (var interceptorType in interceptorTypes)
                {
                    if (interceptorType == null) continue;
                    if (!interceptorType.IsClass)
                    {
                        throw new ArgumentException($"The type \"{interceptorType.FullName}\" should be a class type.");
                    }
                    if (interceptorType.IsAbstract)
                    {
                        throw new ArgumentException($"The type \"{interceptorType.FullName}\" should not be an abstract type.");
                    }
                    if (!typeof(IInterceptor).IsAssignableFrom(interceptorType))
                    {
                        throw new ArgumentException($"The type \"{interceptorType.FullName}\" should not be a sub type from \"{typeof(IInterceptor).FullName}\".");
                    }
                    if (interceptorType.GetConstructor(Type.EmptyTypes) == null)
                    {
                        throw new ArgumentException($"The type \"{interceptorType.FullName}\" should have empty types constructor..");
                    }
                }
            }
        }

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
                var method = typeof(EFContextAttribute)
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

        private void AddDbContext2<InjectType, ImplType>(IServiceCollection services, string connectionString, Type[] interceptorTypes)
            where InjectType : class
            where ImplType : DbContext, InjectType
        {

            var interceptors = (interceptorTypes ?? Type.EmptyTypes).Distinct().Select(p => Activator.CreateInstance(p)).OfType<IInterceptor>();
            services.AddDbContextPool<InjectType, ImplType>((build) =>
            {
                build.AddInterceptors(interceptors);
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
