using Microsoft.Extensions.DependencyInjection;
namespace YS.Knife.TestData
{
    public class MutilAttribute : KnifeAttribute
    {
        public override void RegisteService(KnifeServiceContext registerContext)
        {
            registerContext.Services.AddSingleton(registerContext.InstanceType);
        }
    }
}