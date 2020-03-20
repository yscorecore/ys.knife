using Microsoft.Extensions.DependencyInjection;
using System;

namespace YS.Knife.TestData
{
    public class MutilAttribute : KnifeAttribute
    {
        public override void RegisteService(KnifeRegisteContext registerContext,Type declareType)
        {
            registerContext.Services.AddSingleton(declareType);
        }
    }
}