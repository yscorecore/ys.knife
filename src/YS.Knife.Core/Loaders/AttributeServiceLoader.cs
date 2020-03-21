using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
namespace YS.Knife.Loaders
{
    public class AttributeServiceLoader : IServiceLoader
    {
        public virtual void LoadServices(IServiceCollection services, IRegisteContext context)
        {
            foreach (var type in AppDomain.CurrentDomain.FindInstanceTypesByAttribute<KnifeAttribute>())
            {
                if (context.HasFiltered(type)) continue;
                foreach (var injectAttribute in type.GetCustomAttributes(typeof(KnifeAttribute), true).Cast<KnifeAttribute>())
                {
                    injectAttribute.RegisteService(services, context, type);
                }
            }
        }


    }
}
