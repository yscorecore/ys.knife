using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using YS.Knife;

namespace SwaggerDemo.App
{
    public class SwaggerServiceRegister : IServiceRegister
    {
        public void RegisteServices(IServiceCollection services, IRegisteContext context)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                var basePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                var xmlPath = Path.Combine(basePath, "SwaggerDemo.Api/SwaggerDemo.Api.xml");
                c.IncludeXmlComments(xmlPath);
            });
        }
    }
}
