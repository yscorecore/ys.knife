using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using YS.Knife.Hosting.Web.Swagger;

namespace YS.Knife.Hosting.Web
{
    [SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
    public class DefaultStartup
    {

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                // disable auto validate model
                options.SuppressModelStateInvalidFilter = true;
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            UseStaticFiles(app);

            app.UseKnifeSwagger();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        private void UseStaticFiles(IApplicationBuilder app)
        {
            var knifeOptions = app.ApplicationServices.GetService<IOptions<KnifeWebOptions>>();

            foreach (var kv in knifeOptions.Value.StaticFiles ?? new Dictionary<string, StaticFileInfo>())
            {
                string fullPath = System.IO.Path.GetFullPath(kv.Value.FolderPath);
                var requestPath = new PathString(kv.Key.StartsWith('/') ? kv.Key : '/' + kv.Key);
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(fullPath),
                    ServeUnknownFileTypes = kv.Value.ServeUnknownFileTypes,
                    DefaultContentType = kv.Value.DefaultContentType,
                    RequestPath = requestPath
                });
                if (kv.Value.EnableDirectoryBrowsing)
                {
                    app.UseDirectoryBrowser(new DirectoryBrowserOptions
                    {
                        FileProvider = new PhysicalFileProvider(fullPath),
                        RequestPath = requestPath
                    });
                }
            }
        }



    }
}
