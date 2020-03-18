using Microsoft.Extensions.DependencyInjection;
namespace YS.Knife.TestData
{
    public class MutilAttribute : KnifeAttribute
    {
        public override void RegisteService(KnifeRegisteContext registerContext)
        {
            registerContext.Services.AddSingleton(registerContext.DeclaredType);
        }
    }
}