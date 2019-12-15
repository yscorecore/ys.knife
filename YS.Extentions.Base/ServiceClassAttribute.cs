using Microsoft.Extensions.DependencyInjection;

namespace System
{

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ServiceClassAttribute : Attribute
    {
        public ServiceClassAttribute()
        {

        }
        public ServiceClassAttribute(Type injectType, ServiceLifetime serviceLifetime= ServiceLifetime.Singleton)
        {
            this.InjectType = injectType;
            this.Lifetime = serviceLifetime;
        }
        public Type InjectType { get; private set; }
        public ServiceLifetime Lifetime { get; set; }
    }
}
