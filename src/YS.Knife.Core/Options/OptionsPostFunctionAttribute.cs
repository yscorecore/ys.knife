using System;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.Options
{
    public class OptionsPostFunctionAttribute : KnifeAttribute
    {
        public OptionsPostFunctionAttribute() : base(typeof(IOptionsPostFunction))
        {
        }
        public override void RegisteService(IServiceCollection services, IRegisteContext context, Type declareType)
        {
            services.AddSingleton(typeof(IOptionsPostFunction), declareType);
        }
    }
}
