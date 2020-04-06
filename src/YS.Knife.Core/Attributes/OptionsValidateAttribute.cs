using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace YS.Knife
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class OptionsValidateAttribute : KnifeAttribute
    {
        public OptionsValidateAttribute() : base(typeof(IValidateOptions<>))
        {
        }
        public override void RegisteService(IServiceCollection services, IRegisteContext context, Type declareType)
        {
            var optionsType = FindOptionsType(declareType);
            services.AddSingleton(typeof(IValidateOptions<>).MakeGenericType(optionsType), declareType);
        }
        private Type FindOptionsType(Type declareType)
        {
            foreach (var interfaceType in declareType.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IValidateOptions<>))
                {
                    return interfaceType.GetGenericArguments()[0];
                }
            }
            return null;
        }
    }
}