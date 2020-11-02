using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YS.Knife.Hosting.Web;

namespace YS.Knife.Hosting
{
    public class KnifeWebHost : KnifeHost
    {
        public KnifeWebHost() : base()
        {
        }

        public KnifeWebHost(string[] args, Action<HostBuilderContext, IServiceCollection> configureDelegate = null) :
            base(args, configureDelegate)
        {
        }

        public KnifeWebHost(IDictionary<string, object> args,
            Action<HostBuilderContext, IServiceCollection> configureDelegate = null) : base(args, configureDelegate)
        {
        }

        protected override void OnConfigureCustomService(HostBuilderContext builder, IServiceCollection serviceCollection)
        {
            base.OnConfigureCustomService(builder, serviceCollection);
            serviceCollection.AddSingleton(typeof(KnifeWebHost), this);
        }

        protected virtual void ConfigureWebApp(IApplicationBuilder app, IWebHostEnvironment env)
        {
            AppConfiguration.ConfigureDefaultWebApp(app, env);
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            return base.CreateHostBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<CommonStartup>();
                });
        }



        protected class CommonStartup
        {
            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                var webHost = app.ApplicationServices.GetRequiredService<KnifeWebHost>();
                webHost.ConfigureWebApp(app, env);
            }

            public void ConfigureServices(IServiceCollection services)
            {
                // services.Configure<ApiBehaviorOptions>(options =>
                // {
                //     // disable auto validate model
                //     options.SuppressModelStateInvalidFilter = true;
                // });
            }
        }
    }
}
