using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace YS.Knife.Rest.AspNetCore
{
    class ServiceRegister : IServiceRegister
    {
        public void RegisterServices(IServiceCollection services, IRegisterContext context)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<IApplicationModelProvider, CodeResultApplicationModelProvider>());

        }
    }
}
