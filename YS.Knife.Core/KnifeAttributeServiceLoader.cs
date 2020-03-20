using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
namespace YS.Knife
{
    public class KnifeAttributeServiceLoader : IServiceLoader
    {
        public virtual void LoadServices(IServiceCollection services, IConfiguration configuration)
        {
            foreach (var type in AppDomain.CurrentDomain.FindInstanceTypesByAttribute<KnifeAttribute>())
            {
                if (services.IsFilter(type, configuration)) continue;
                foreach (var injectAttribute in type.GetCustomAttributes(typeof(KnifeAttribute), true).Cast<KnifeAttribute>())
                {
                    injectAttribute.RegisteService(
                        new KnifeRegisteContext
                        {
                            Services = services,
                            Configuration = configuration,

                        }, type);
                }
            }
        }

    }
}