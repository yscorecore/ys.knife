using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
namespace YS.Knife.Registers
{
    public class AttributeServiceRegister : IServiceRegister
    {
        public virtual void RegisterServices(IServiceCollection services, IRegisteContext context)
        {
            foreach (var type in AppDomain.CurrentDomain.FindInstanceTypesByAttribute<KnifeAttribute>())
            {
                if (context.HasFiltered(type)) continue;
                foreach (var injectAttribute in type.GetCustomAttributes(typeof(KnifeAttribute), true).Cast<KnifeAttribute>())
                {
                    if (injectAttribute.ValidateFromType != null)
                    {
                        if (injectAttribute.ValidateFromType.IsGenericTypeDefinition)
                        {
                            if (!IsAssignableFromGenericType(type, injectAttribute.ValidateFromType))
                            {
                                throw new InvalidOperationException($"The type '{type.FullName}' must be a child class from '{injectAttribute.ValidateFromType.FullName}'.");
                            }
                        }
                        else
                        {
                            if (!injectAttribute.ValidateFromType.IsAssignableFrom(type))
                            {
                                throw new InvalidOperationException($"The type '{type.FullName}' must be a child class from '{injectAttribute.ValidateFromType.FullName}'.");
                            }
                        }
                    }
                    injectAttribute.RegisterService(services, context, type);
                }
            }
        }

        private bool IsAssignableFromGenericType(Type type, Type genericTypeDefinition)
        {
            if (!genericTypeDefinition.IsGenericTypeDefinition) return false;
            //base classes
            var tempType = type;
            while (tempType != null)
            {
                if (tempType.IsGenericType && tempType.GetGenericTypeDefinition() == genericTypeDefinition)
                {
                    return true;
                }
                tempType = tempType.BaseType;
            }
            //interfaces
            foreach (var interfaceType in type.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == genericTypeDefinition)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
