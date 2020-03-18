using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
namespace YS.Knife.Core
{
    public class KnifeAttributeServiceLoader : IServiceLoader
    {
        public virtual void LoadServices(IServiceCollection services, IConfiguration configuration)
        {
            foreach (var type in AppDomain.CurrentDomain.FindInstanceTypesByAttribute<KnifeAttribute>())
            {
                if (ShouldFilter(type)) continue;
                foreach (var injectAttribute in type.GetCustomAttributes(typeof(KnifeAttribute), true).Cast<KnifeAttribute>())
                {
                    injectAttribute.RegisteService(
                        new KnifeServiceContext
                        {
                            Services = services,
                            Configuration = configuration,
                            InstanceType = type
                        });
                }
            }
        }
        protected virtual bool ShouldFilter(Type type)
        {
            return false;
        }
    }
}