using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace YS.Knife
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class OptionsValidateHandlerAttribute : KnifeAttribute
    {
        public OptionsValidateHandlerAttribute() : base(typeof(IValidateOptions<>))
        {
        }
        public override void RegisteService(IServiceCollection services, IRegisteContext context, Type declareType)
        {
            _ = declareType ?? throw new ArgumentNullException(nameof(declareType));
            var optionsType = FindOptionsType(declareType);
            services.AddSingleton(typeof(IValidateOptions<>).MakeGenericType(optionsType), declareType);
        }
        private Type FindOptionsType(Type declareType)
        {
            return declareType.GetInterfaces()
                 .Where(p => p.IsGenericType && p.GetGenericTypeDefinition() == typeof(IValidateOptions<>))
                 .Select(p => p.GetGenericArguments().First())
                 .FirstOrDefault();
        }
    }
}
