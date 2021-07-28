using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
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

        public KnifeWebHost(string[] args) :
            base(args)
        {
        }

        public KnifeWebHost(IDictionary<string, object> args) : base(args)
        {
        }


        protected override void OnConfigureInternalServices(HostBuilderContext builder,
            IServiceCollection serviceCollection)
        {
            base.OnConfigureInternalServices(builder, serviceCollection);
            serviceCollection.AddSingleton(typeof(KnifeWebHost), this);
        }
        protected virtual void OnPreConfigureWebApp(IApplicationBuilder app, IWebHostEnvironment env)
        {


        }
        protected virtual void OnConfigureWebApp(IApplicationBuilder app, IWebHostEnvironment env)
        {
            AppConfiguration.ConfigureDefaultWebApp(app, env);
        }

        protected virtual void OnPostConfigureWebApp(IApplicationBuilder app, IWebHostEnvironment env)
        {


        }



        protected virtual void OnConfigureWebHostBuilder(IWebHostBuilder webBuilder)
        {

        }

        protected override IHostBuilder CreateHostBuilder(string[] args)
        {
            return base.CreateHostBuilder(args)
                .ConfigureWebHostDefaults((webBuilder) =>
                {
                    webBuilder.UseStartup<CommonStartup>();
                    OnConfigureWebHostBuilder(webBuilder);
                });
        }



        protected class CommonStartup
        {
            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                _ = app ?? throw new ArgumentNullException(nameof(app));
                var webHost = app.ApplicationServices.GetRequiredService<KnifeWebHost>();
                webHost.OnPreConfigureWebApp(app, env);
                webHost.OnConfigureWebApp(app, env);
                webHost.OnPostConfigureWebApp(app, env);
            }

            public void ConfigureServices(IServiceCollection _)
            {
            }
        }
    }
}
