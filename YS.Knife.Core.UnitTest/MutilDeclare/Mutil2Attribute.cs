using Microsoft.Extensions.DependencyInjection;
namespace YS.Knife.MutilDeclare
{
    public class Mutil2Attribute : KnifeAttribute
    {
        public override void RegisteService(KnifeServiceContext registerContext)
        {
            registerContext.Services.AddSingleton(registerContext.InstanceType);
        }
    }
}