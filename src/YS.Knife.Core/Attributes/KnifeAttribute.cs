using Microsoft.Extensions.DependencyInjection;
using System;

namespace YS.Knife
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public abstract class KnifeAttribute : Attribute
    {
        public abstract void RegisteService(IServiceCollection services, IRegisteContext context, Type declareType);
    }

    public static class KnifeAttributeExtensions
    {
        public static void ValidateType(this KnifeAttribute attribute, Type declareType, Type fromType)
        {
            _ = declareType ?? throw new ArgumentNullException(nameof(declareType));
            _ = fromType ?? throw new ArgumentNullException(nameof(fromType));
            if (!fromType.IsAssignableFrom(declareType))
            {
                throw new InvalidOperationException($"The {declareType.FullName} does not from {fromType.FullName}.");
            }
        }
    }

}
