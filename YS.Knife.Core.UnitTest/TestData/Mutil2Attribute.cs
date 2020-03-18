using Microsoft.Extensions.DependencyInjection;
namespace YS.Knife.TestData
{
    public class Mutil2Attribute : KnifeAttribute
    {
        public override void RegisteService(KnifeRegisteContext registerContext)
        {
            registerContext.Services.AddSingleton(registerContext.DeclaredType);
        }
    }
}