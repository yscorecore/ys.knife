using System;
using Microsoft.Extensions.DependencyInjection;
namespace YS.Knife
{
    public class ResourceAttribute : KnifeAttribute
    {
        public ResourceAttribute() : base(null)
        {
        }
        public override void RegisteService(IServiceCollection services, IRegisteContext context, Type declareType)
        {
            services.AddSingleton(declareType);
        }
    }
}
