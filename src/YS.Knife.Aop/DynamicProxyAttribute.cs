using System;
using System.Collections.Generic;
using System.Text;
using AspectCore.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.Aop
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public abstract class DynamicProxyAttribute : Attribute
    {
        public DynamicProxyAttribute(ServiceLifetime lifetime)
        {
            this.Lifetime = lifetime;
        }

        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Scoped;


        public virtual void RegisterService(IServiceCollection services, IRegisterContext context, Type declareType)
        {
            services.Add(new ServiceDescriptor(declareType, sp => CreateInterfaceDynamicObject(sp, declareType), this.Lifetime));
        }
        private object CreateInterfaceDynamicObject(IServiceProvider serviceProvider, Type interfaceType)
        {
            var proxyGenerator = serviceProvider.GetRequiredService<IProxyGenerator>();
            return proxyGenerator.CreateInterfaceProxy(interfaceType);
        }
    }
}
