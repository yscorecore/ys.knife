using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Knife.Hosting
{
    public class KnifeWebHost<TStartup> : KnifeHost
        where TStartup : class
    {
        public KnifeWebHost() : base()
        {

        }
        public KnifeWebHost(string[] args, Action<HostBuilderContext, IServiceCollection> configureDelegate = null) : base(args, configureDelegate)
        {

        }
        public KnifeWebHost(IDictionary<string, object> args, Action<HostBuilderContext, IServiceCollection> configureDelegate = null) : base(args, configureDelegate)
        {

        }
        protected override IHostBuilder CreateHostBuilder()
        {
            return base.CreateHostBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<TStartup>();
                });
        }
        protected override void LoadCustomService(HostBuilderContext builder, IServiceCollection serviceCollection)
        {
            this.LoadWebApiService(builder, serviceCollection);
            base.LoadCustomService(builder, serviceCollection);
        }

        private void LoadWebApiService(HostBuilderContext builder, IServiceCollection serviceCollection)
        {
            var options = builder.Configuration.GetConfigOrNew<WebAppOptions>();
            IMvcBuilder mvcBuilder = serviceCollection.AddControllers((mvc) =>
            {
            });
            var parts = from p in AppDomain.CurrentDomain.GetAssemblies()
                        where p.GetName().Name.IsMatchWildcardAnyOne(options.MvcParts, StringComparison.OrdinalIgnoreCase)
                        select p;
            foreach (var mvcPart in parts)
            {
                mvcBuilder.AddApplicationPart(mvcPart);
            }
        }
    }

    public class KnifeWebHost : KnifeWebHost<Startup>
    {
        public KnifeWebHost()
        {

        }
        public KnifeWebHost(IDictionary<string, object> args, Action<HostBuilderContext, IServiceCollection> configureDelegate = null) : base(args, configureDelegate)
        {

        }
        public KnifeWebHost(string[] args, Action<HostBuilderContext, IServiceCollection> configureDelegate = null) : base(args, configureDelegate)
        {

        }

        #region Static

        public new static void Start(string[] args, Action<HostBuilderContext, IServiceCollection> configureDelegate = null)
        {
            new KnifeWebHost(args, configureDelegate).Run();
        }
        #endregion
    }
}
