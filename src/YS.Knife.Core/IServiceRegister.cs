using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife
{
    public interface IServiceRegister
    {
        void RegisteServices(IServiceCollection services, IRegisteContext context);
    }
}
