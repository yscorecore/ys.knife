using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YS.Knife;
using YS.Knife.EntityFrameworkCore;

namespace Microsoft.EntityFrameworkCore
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class KnifeEFContextAttribute : KnifeAttribute
    {

        public KnifeEFContextAttribute() : base(typeof(DbContext))
        {
        }

        public bool RegisterEntityStore { get; set; } = true;
        public bool EnablePool { get; set; } = false;
        public int PoolSize { get; set; } = 128;
        public override void RegisterService(IServiceCollection services, IRegisterContext context, Type declareType)
        {
            _ = declareType ?? throw new ArgumentNullException(nameof(declareType));
            _ = context ?? throw new ArgumentNullException(nameof(context));

            var method = typeof(KnifeEFContextAttribute)
                   .GetMethod(nameof(AddDbContext2), BindingFlags.Instance | BindingFlags.NonPublic)
                   ?.MakeGenericMethod(declareType);
            method.Invoke(this, new object[] { services });

            if (RegisterEntityStore)
            {
                AddEntityStoresInternal(services, declareType);
            }
        }

        private void AddDbContext2<ImplType>(IServiceCollection services)
            where ImplType : DbContext
        {
            if (EnablePool && PoolSize>0)
            {
                services.AddDbContextPool<ImplType>((sp, builder) =>
                {
                    var configration = sp.GetRequiredService<DbContextConfigration<ImplType>>();
                    configration?.ConfigOptions(sp, builder);
                    
                }, PoolSize);
            }
            else
            {
                services.AddDbContext< ImplType>((sp, builder) =>
                {
                    var configration = sp.GetRequiredService<DbContextConfigration<ImplType>>();
                    configration?.ConfigOptions(sp, builder);
                });
            }
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
    }
}

