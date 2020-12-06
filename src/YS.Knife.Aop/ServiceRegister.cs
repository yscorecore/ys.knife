using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.Aop
{
    public class ServiceRegister : IServiceRegister
    {
        public void RegisterServices(IServiceCollection services, IRegisterContext context)
        {
            services.ConfigureDynamicProxy(config =>
            {
                config.ThrowAspectException = false;
            });
        }
    }
}
