using System;
using Microsoft.Extensions.DependencyInjection;
namespace YS.Knife
{
    public class ResourceAttribute : KnifeAttribute
    {
        public ResourceAttribute() : base(null)
        {
        }
        public override void RegisterService(IServiceCollection services, IRegisterContext context, Type declareType)
        {
            services.AddSingleton(declareType);
        }
    }
}
