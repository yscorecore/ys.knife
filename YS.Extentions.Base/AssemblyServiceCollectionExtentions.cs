using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace System
{
    public static class AssemblyServiceCollectionExtentions
    {
        public static Assembly AppendServices(this Assembly assembly,IServiceCollection services, IConfiguration configuration)
        {
            var serviceTypes = from p in assembly.GetTypes()
                               where Attribute.IsDefined(p, typeof(ServiceImplClass))
                                     && !p.IsAbstract
                               select p;
            foreach (var serviceType in serviceTypes)
            {
                var serviceAttr = Attribute.GetCustomAttribute(serviceType, typeof(ServiceImplClass)) as ServiceImplClass;

                switch (serviceAttr.Lifetime)
                {
                    case ServiceLifetime.Singleton:
                        services.AddSingleton(serviceAttr.InjectType, serviceType);
                        break;
                    case ServiceLifetime.Scoped:
                        services.AddScoped(serviceAttr.InjectType, serviceType);
                        break;
                    case ServiceLifetime.Transient:
                        services.AddTransient(serviceAttr.InjectType, serviceType);
                        break;
                }
            }
            return assembly;
        }

        public static Assembly AppendLoadServices(this Assembly assembly, IServiceCollection services, IConfiguration configuration)
        {
            var loadTypes = from p in assembly.GetTypes()
                               where typeof(IServiceLoader).IsAssignableFrom(p)
                                     && !p.IsAbstract && p.GetConstructor(Type.EmptyTypes)!=null
                               select p;
            foreach (var loadType in loadTypes)
            {
                IServiceLoader loader = Activator.CreateInstance(loadType) as IServiceLoader;
                loader.LoadServices(services, configuration);
            }
            return assembly;
        }
    }
}
