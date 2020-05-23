using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
namespace YS.Knife
{

    public class ServiceClassAttribute : KnifeAttribute
    {
        public ServiceClassAttribute() : base(null)
        {
        }
        public ServiceClassAttribute(Type injectType, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped) : base(null)
        {
            this.InjectType = injectType;
            this.Lifetime = serviceLifetime;
        }
        public Type InjectType { get; private set; }
        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Scoped;
        public override void RegisteService(IServiceCollection services, IRegisteContext context, Type declareType)
        {
            _ = context ?? throw new ArgumentNullException(nameof(declareType));
            _ = declareType ?? throw new ArgumentNullException(nameof(declareType));
            var injectType = this.InjectType ?? DeduceInjectType(declareType);

            switch (this.Lifetime)
            {
                case ServiceLifetime.Singleton:
                    services.AddSingleton(injectType, declareType);
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped(injectType, declareType);
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient(injectType, declareType);
                    break;
            }
        }
        private Type DeduceInjectType(Type serviceType)
        {
            var allInterfaces = serviceType.GetInterfaces();
            if (allInterfaces.Length != 1)
            {
                throw new InvalidOperationException($"Can not deduce the inject type from current type '{serviceType.FullName}'.");
            }
            return allInterfaces.First();
        }


    }
}
