using System;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.TestData
{
    public class MutilAttribute : KnifeAttribute
    {
        public MutilAttribute() : base(default)
        {

        }
        public override void RegisteService(IServiceCollection services, IRegisteContext context, Type declareType)
        {
            services.AddSingleton(declareType);
        }
    }
}
