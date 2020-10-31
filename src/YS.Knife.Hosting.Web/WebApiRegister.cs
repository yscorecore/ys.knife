using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using YS.Knife.Hosting.Web.Filters;

namespace YS.Knife.Hosting.Web
{
    public class WebApiRegister : IServiceRegister
    {
        public void RegisterServices(IServiceCollection services, IRegisteContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));
            RegisterController(services, context);
            RegisterHttpContext(services);
        }

        private void RegisterController(IServiceCollection services, IRegisteContext context)
        {
            var options = context.Configuration.GetConfigOrNew<KnifeWebOptions>();
            IMvcBuilder mvcBuilder = services.AddControllers((mvc) =>
            {
                if (options.WrapCodeMessageResult)
                {
                    mvc.Filters.Add(typeof(WrapCodeMessageAttribute));
                }
                // if the Accept type not provide, return 406 code
                mvc.ReturnHttpNotAcceptable = true;
            }).AddXmlDataContractSerializerFormatters();

            var controllerAssemblies = AppDomain.CurrentDomain.FindInstanceTypesByAttribute<ControllerAttribute>()
                .Select(p => p.Assembly)
                .Distinct();
            foreach (var mvcPart in controllerAssemblies)
            {
                mvcBuilder.AddApplicationPart(mvcPart);
            }
        }

        private void RegisterHttpContext(IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }
    }
}
