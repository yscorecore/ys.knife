using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SwaggerDemo.App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            UseSwagger(app);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void UseSwagger(IApplicationBuilder app)
        {
            var swaggerOptions = app.ApplicationServices.GetRequiredService<SwaggerOptions>();
            var ui = swaggerOptions.UI ?? new UIInfo();
            var api = swaggerOptions.Api ?? new ApiInfo();
            app.UseSwagger(c => { c.RouteTemplate = api.RouteTemplate; });
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = swaggerOptions.UI.RoutePrefix;
                var documentName = swaggerOptions.GetDocumentNameOrEntryAssemblyName();
                string swaggerPath =  api.RouteTemplate.Replace("{documentName}", UrlEncoder.Default.Encode(documentName));
                c.SwaggerEndpoint(swaggerPath, documentName);

            });
        }
    }
}
