using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.Hosting.Web
{
    public class WebApiRegister : IServiceRegister
    {
        public void RegisteServices(IServiceCollection services, IRegisteContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));
            var options = context.Configuration.GetConfigOrNew<KnifeWebOptions>();
            IMvcBuilder mvcBuilder = services.AddControllers((mvc) =>
            {
            });
            var parts = from p in AppDomain.CurrentDomain.GetAssemblies()
                        where p.GetName().Name.IsMatchWildcardAnyOne(options.MvcParts, StringComparison.OrdinalIgnoreCase)
                        select p;
            foreach (var mvcPart in parts)
            {
                mvcBuilder.AddApplicationPart(mvcPart);
            }
        }
    }
}
