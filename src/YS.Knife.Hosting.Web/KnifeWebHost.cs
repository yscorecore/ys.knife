using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YS.Knife.Hosting.Web;

namespace YS.Knife.Hosting
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

    }

    public class KnifeWebHost : KnifeWebHost<DefaultStartup>
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

        public static new void Start(string[] args, Action<HostBuilderContext, IServiceCollection> configureDelegate = null)
        {
            using (var knifeHost = new KnifeWebHost(args, configureDelegate))
            {
                knifeHost.Run();
            }
        }
        #endregion
    }
}
