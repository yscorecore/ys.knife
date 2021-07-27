using System;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.Hosting.Web.Swagger
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseKnifeSwagger(this IApplicationBuilder app)
        {
            _ = app ?? throw new ArgumentNullException(nameof(app));
            var swaggerOptions = app.ApplicationServices.GetRequiredService<SwaggerOptions>();
            var ui = swaggerOptions.UI ?? new UIInfo();
            var api = swaggerOptions.Api ?? new ApiInfo();
            if (swaggerOptions.Mode == Mode.Api || swaggerOptions.Mode == Mode.All)
            {
                app.UseSwagger(c => { c.RouteTemplate = api.RouteTemplate; });
            }
            if (swaggerOptions.Mode == Mode.All)
            {
                app.UseSwaggerUI(c =>
                {
                    c.RoutePrefix = swaggerOptions.UI.RoutePrefix;
                    var documentName = swaggerOptions.GetDocumentNameOrEntryAssemblyName();
                    string swaggerPath = api.RouteTemplate.Replace("{documentName}", UrlEncoder.Default.Encode(documentName), StringComparison.InvariantCultureIgnoreCase);
                    c.SwaggerEndpoint(swaggerPath, documentName);
                    if (!string.IsNullOrEmpty(ui.CssUrl))
                    {
                        c.InjectStylesheet(ui.CssUrl);
                    }
                    if (!string.IsNullOrEmpty(ui.JavascriptUrl))
                    {
                        c.InjectJavascript(ui.JavascriptUrl);
                    }
                   
                });
            }
        }
    }
}
