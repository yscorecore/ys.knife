using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife
{
    public class DefaultServiceRegister : IServiceRegister
    {
        public void RegisterServices(IServiceCollection services, IRegisteContext context)
        {
            services.AddOptions();
            services.AddLogging();
            services.AddLocalization();
        }
    }
}
