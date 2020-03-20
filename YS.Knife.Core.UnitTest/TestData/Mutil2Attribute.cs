using Microsoft.Extensions.DependencyInjection;
using System;

namespace YS.Knife.TestData
{
    public class Mutil2Attribute : KnifeAttribute
    {
        public override void RegisteService(KnifeRegisteContext registerContext,Type declareType)
        {
            registerContext.Services.AddSingleton(declareType);
        }
    }
}