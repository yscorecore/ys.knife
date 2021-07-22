using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
namespace YS.Knife
{

    public class ServiceAttribute : KnifeAttribute
    {
        public ServiceAttribute() : base(null)
        {
        }
        public ServiceAttribute(Type injectType, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped) : base(null)
        {
            this.InjectType = injectType;
            this.Lifetime = serviceLifetime;
        }
        public Type InjectType { get; private set; }
        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Scoped;
        public override void RegisterService(IServiceCollection services, IRegisterContext context, Type declareType)
        {
            _ = context ?? throw new ArgumentNullException(nameof(declareType));
            _ = declareType ?? throw new ArgumentNullException(nameof(declareType));
            var injectType = this.InjectType ?? DeduceInjectType(declareType);
            services.Add(new ServiceDescriptor(injectType, declareType, this.Lifetime));
            if (injectType != declareType)
            {
                services.Add(new ServiceDescriptor(declareType, declareType, this.Lifetime));
            }
        }
        private static Type DeduceInjectType(Type serviceType)
        {
            var allInterfaces = serviceType.GetInterfaces();
            if (allInterfaces.Length == 0)
            {
                return serviceType;
            }

            if (allInterfaces.Length == 1)
            {
                return allInterfaces.First();
            }

            throw new InvalidOperationException($"Can not deduce the inject type from current type '{serviceType.FullName}', found too many interfaces, please set the InjectType manually.");
        }


    }
}
