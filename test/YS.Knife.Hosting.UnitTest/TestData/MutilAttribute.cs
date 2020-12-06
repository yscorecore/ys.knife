using System;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.TestData
{
    public class MultiAttribute : KnifeAttribute
    {
        public MultiAttribute() : base(default)
        {

        }
        public override void RegisterService(IServiceCollection services, IRegisterContext context, Type declareType)
        {
            services.AddSingleton(declareType);
        }
    }
}
