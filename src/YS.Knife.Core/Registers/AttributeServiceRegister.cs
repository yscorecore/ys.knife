﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
namespace YS.Knife.Loaders
{
    public class AttributeServiceRegister : IServiceRegister
    {
        public virtual void RegisteServices(IServiceCollection services, IRegisteContext context)
        {
            foreach (var type in AppDomain.CurrentDomain.FindInstanceTypesByAttribute<KnifeAttribute>())
            {
                if (context.HasFiltered(type)) continue;
                foreach (var injectAttribute in type.GetCustomAttributes(typeof(KnifeAttribute), true).Cast<KnifeAttribute>())
                {
                    if (injectAttribute.ValidateFromType != null && !injectAttribute.ValidateFromType.IsAssignableFrom(type))
                    {
                        throw new InvalidOperationException($"The type '{type.FullName}' must be a child class from '{injectAttribute.ValidateFromType.FullName}'.");
                    }
                    injectAttribute.RegisteService(services, context, type);
                }
            }
        }


    }
}
