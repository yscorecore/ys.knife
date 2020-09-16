using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.Aop
{
    public class ServiceRegister : IServiceRegister
    {
        public void RegisterServices(IServiceCollection services, IRegisteContext context)
        {
            services.ConfigureDynamicProxy(config =>
            {
                config.ThrowAspectException = false;
            });
        }
    }
}
