using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.Aop
{
   public  class DynamicServiceRegister : IServiceRegister
    {
        public void RegisterServices(IServiceCollection services, IRegisterContext context)
        {
            foreach (var type in AppDomain.CurrentDomain.FindInterfaceTypesByAttribute<DynamicProxyAttribute>())
            {
                if (context.HasFiltered(type)) continue;

                var dynamicImplmentAttributes = type.GetCustomAttributes<DynamicProxyAttribute>(true).ToList();
                if (dynamicImplmentAttributes.Count > 1)
                {
                    throw new NotSupportedException($"Only one DynamicProxyAttribute can be used for the interface '{type.FullName}'.");
                }
                dynamicImplmentAttributes.First().RegisterService(services, context, type);

            }
        }
    }
}
