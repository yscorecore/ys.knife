using System;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife
{

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ServiceClassAttribute : Attribute
    {
        public ServiceClassAttribute()
        {

        }
        public ServiceClassAttribute(Type injectType, ServiceLifetime serviceLifetime= ServiceLifetime.Scoped)
        {
            this.InjectType = injectType;
            this.Lifetime = serviceLifetime;
        }
        public Type InjectType { get; private set; }
        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Scoped;

        public string Key { get; set; }
    }
}
