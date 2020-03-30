using System;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.Stage
{
    public class StageAttribute : KnifeAttribute
    {
        public StageAttribute() : base(typeof(IStageService))
        {
        }

        public override void RegisteService(IServiceCollection services, IRegisteContext context, Type declareType)
        {
            services.AddTransient(typeof(IStageService), declareType);
        }
    }
}