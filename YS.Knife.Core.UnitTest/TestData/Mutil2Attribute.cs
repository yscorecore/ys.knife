using Microsoft.Extensions.DependencyInjection;
using System;

namespace YS.Knife.TestData
{
    public class Mutil2Attribute : KnifeAttribute
    {
        public override void RegisteService(IServiceCollection services, IRegisteContext context,Type declareType)
        {
            services.AddSingleton(declareType);
        }
    }
}