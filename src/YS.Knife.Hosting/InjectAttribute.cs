using System;
using Microsoft.Extensions.DependencyInjection;
namespace YS.Knife.Hosting
{
    [System.AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class InjectAttribute : System.Attribute
    {
        public InjectAttribute(ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            this.Lifetime = lifetime;
        }
        public ServiceLifetime Lifetime { get; set; }
    }
}
