using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace YS.Knife.Hosting.Web.Swagger
{
    public class SwaggerServiceRegister : IServiceRegister
    {
        public void RegisterServices(IServiceCollection services, IRegisterContext context)
        {
            var swaggerOptions = context?.Configuration.GetConfigOrNew<SwaggerOptions>();
            if (swaggerOptions.Mode == Mode.None) return;
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

                    foreach (var file in Directory.GetFiles(basePath, api.XmlCommentsFiles))
                    {
                        c.IncludeXmlComments(file);
                    };
                }
            });
        }
    }
}
