using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using YS.Knife.Aop.Logging;

namespace YS.Knife.Aop
{
    public class ServiceRegister : IServiceRegister
    {
        public void RegisteServices(IServiceCollection services, IRegisteContext context)
        {
            //services.AddTransient<LoggingAttribute>();

            //services.ConfigureDynamicProxy(config =>
            //{
            //    // 加入已注册服务
            //    config.Interceptors.AddServiced<LoggingAttribute>();
            //});
            services.ConfigureDynamicProxy(config =>
            {
                config.ThrowAspectException = false;
            });
        }
    }
}
