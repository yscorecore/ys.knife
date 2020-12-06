using System;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.TestData
{
    public class Multi2Attribute : KnifeAttribute
    {
        public Multi2Attribute() : base(default)
        {

        }
        public override void RegisterService(IServiceCollection services, IRegisterContext context, Type declareType)
        {
            services.AddSingleton(declareType);
        }
    }
}
