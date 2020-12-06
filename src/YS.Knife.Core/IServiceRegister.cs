using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife
{
    public interface IServiceRegister
    {
        void RegisterServices(IServiceCollection services, IRegisterContext context);
    }
}
