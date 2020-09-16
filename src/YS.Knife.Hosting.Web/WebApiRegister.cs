using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.Hosting.Web
{
    public class WebApiRegister : IServiceRegister
    {
        public void RegisterServices(IServiceCollection services, IRegisteContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));
            var options = context.Configuration.GetConfigOrNew<KnifeWebOptions>();
            IMvcBuilder mvcBuilder = services.AddControllers((mvc) =>
            {
            });

            var controllerAssemblies = AppDomain.CurrentDomain.FindInstanceTypesByAttribute<ControllerAttribute>()
                                    .Select(p => p.Assembly)
                                    .Distinct();
            foreach (var mvcPart in controllerAssemblies)
            {
                mvcBuilder.AddApplicationPart(mvcPart);
            }
        }
    }
}
