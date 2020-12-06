using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife
{
    public class DefaultServiceRegister : IServiceRegister
    {
        public void RegisterServices(IServiceCollection services, IRegisterContext context)
        {
            services.AddOptions();
            services.AddLogging();
            services.AddLocalization();
        }
    }
}
