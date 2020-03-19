using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Generic;
namespace YS.Knife
{

    public class ServiceClassAttribute : KnifeAttribute
    {
        public ServiceClassAttribute()
        {

        }
        public ServiceClassAttribute(Type injectType, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            this.InjectType = injectType;
            this.Lifetime = serviceLifetime;
        }
        public Type InjectType { get; private set; }
        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Scoped;

        public string Key { get; set; }

        public override void RegisteService(KnifeRegisteContext context)
        {
            var injectType = this.InjectType ?? DeduceInjectType(context.DeclaredType);
            var dictionaryType = typeof(IDictionary<,>).MakeGenericType(typeof(string), injectType);
          
            switch (this.Lifetime)
            {
                case ServiceLifetime.Singleton:
                    context.Services.AddSingleton(injectType, context.DeclaredType);
                    break;
                case ServiceLifetime.Scoped:
                    context.Services.AddScoped(injectType, context.DeclaredType);
                    break;
                case ServiceLifetime.Transient:
                    context.Services.AddTransient(injectType, context.DeclaredType);
                    break;
            }
        }
        private object GetDictionary(IServiceProvider sp, Type injectType)
        {
            return null;
        }
        //private IDictionary<string, T> GetGenericDictionary<T>(IServiceProvider sp)
        //{
        //    return sp.GetService<IEnumerable<T>>()
        //            .Where(p => p != null)
        //            .ToDictionary(p => GetServiceKey(p.GetType()));
        //}

        private Type DeduceInjectType(Type serviceType)
        {
            var allInterfaces = serviceType.GetInterfaces();
            if (allInterfaces.Length != 1)
            {
                throw new InvalidOperationException($"Can not deduce the inject type from current type '{serviceType.FullName}'.");
            }
            return allInterfaces.First();
        }

        protected class KnifeInjectionDictionary<T> : Dictionary<string, T>
        {
            public KnifeInjectionDictionary(IEnumerable<T> items)
            {
                foreach (var item in items)
                {
                    if (item != null)
                    {
                        this[""] = item;
                    }
                }
            }
            private static string GetServiceKey(Type type)
            {
                ServiceClassAttribute serviceImplClass = Attribute.GetCustomAttribute(type, typeof(ServiceClassAttribute)) as ServiceClassAttribute;
                if (serviceImplClass == null || string.IsNullOrEmpty(serviceImplClass.Key))
                {
                    return type.FullName;
                }
                else
                {
                    return serviceImplClass.Key;
                }
            }
        }
    }
}
