using System;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.Stages
{
    public class StageAttribute : KnifeAttribute
    {
        public StageAttribute() : base(typeof(IStageService))
        {
        }

        public override void RegisterService(IServiceCollection services, IRegisteContext context, Type declareType)
        {
            services.AddTransient(typeof(IStageService), declareType);
        }
    }
}
