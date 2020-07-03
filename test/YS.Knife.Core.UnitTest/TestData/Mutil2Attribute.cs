using System;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.TestData
{
    public class Mutil2Attribute : KnifeAttribute
    {
        public Mutil2Attribute() : base(default)
        {

        }
        public override void RegisteService(IServiceCollection services, IRegisteContext context, Type declareType)
        {
            services.AddSingleton(declareType);
        }
    }
}
