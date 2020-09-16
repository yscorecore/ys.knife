using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife
{
    public interface IServiceRegister
    {
        void RegisterServices(IServiceCollection services, IRegisteContext context);
    }
}
