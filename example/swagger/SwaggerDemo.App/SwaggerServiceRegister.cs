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
            var swaggerOptions = context.Configuration.GetConfigOrNew<SwaggerOptions>();
            services.AddSwaggerGen(c =>
            {
                var api = swaggerOptions.Api ?? new ApiInfo();
                var documentName = swaggerOptions.GetDocumentNameOrEntryAssemblyName();
                c.SwaggerDoc(documentName,
                    new OpenApiInfo
                    {
                        Title = api.GetTitleOrAssemblyTitle(),
                        Version = api.GetVersionOrAssemblyVersion(),
                        Description = api.GetDescriptionOrAssemblyDescription()
                    });
                if (api.IncludeXmlComments)
                {
                    var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                    foreach (var file in System.IO.Directory.GetFiles(basePath, api.XmlComentsFiles))
                    {
                        c.IncludeXmlComments(file);
                    };
                    //var xmlPath = Path.Combine(basePath, "SwaggerDemo.Api/SwaggerDemo.Api.xml");
                    //c.IncludeXmlComments(xmlPath);
                }

            });
        }
    }
}
