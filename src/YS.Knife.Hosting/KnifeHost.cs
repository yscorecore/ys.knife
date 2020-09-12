using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YS.Knife.Stage;
namespace YS.Knife.Hosting
{

    public class KnifeHost : IDisposable, IServiceProvider
    {
        public KnifeHost() : this(Array.Empty<string>())
        {
        }
        public KnifeHost(IDictionary<string, object> args, Action<HostBuilderContext, IServiceCollection> configureDelegate = null)
            : this(args?.Select(p => $"/{p.Key}={p.Value}").ToArray(), configureDelegate)
        {
        }
        public KnifeHost(string[] args, Action<HostBuilderContext, IServiceCollection> configureDelegate = null)
        {
            this.args = args;
            this.configureDelegate = configureDelegate;
#pragma warning disable CA2214 // 不要在构造函数中调用可重写的方法
            this.host = CreateHostBuilder().Build();
#pragma warning restore CA2214 // 不要在构造函数中调用可重写的方法
        }

        private readonly Action<HostBuilderContext, IServiceCollection> configureDelegate;
        private readonly string[] args;
        private readonly IHost host;

        public object GetService(Type serviceType)
        {
            return host.Services.GetService(serviceType);
        }
        public T GetService<T>()
        {
            return ServiceProviderServiceExtensions.GetService<T>(this);
        }

        protected virtual IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder(args ?? Array.Empty<string>())
                .UseAopServiceProviderFactory()
                .ConfigureAppConfiguration(OnConfigureAppConfiguration)
                .ConfigureServices((builder, serviceCollection) =>
                {
                    serviceCollection.AddAllKnifeServices(builder.Configuration);
                    this.OnLoadCustomService(builder, serviceCollection);
                });
        }
        protected virtual void OnLoadCustomService(HostBuilderContext builder, IServiceCollection serviceCollection)
        {
            configureDelegate?.Invoke(builder, serviceCollection);
        }
        protected virtual void OnConfigureAppConfiguration(HostBuilderContext hostBuilderContext, IConfigurationBuilder configurationBuilder)
        {

        }


        public void Run()
        {
            var options = this.host.Services.GetService<IOptions<KnifeOptions>>();
            if (string.IsNullOrEmpty(options.Value.Stage))
            {
                this.host.Run();
            }
            else
            {
                this.host.RunStage(options.Value.Stage);
            }
        }
        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    host.Dispose();
                }
                disposedValue = true;
            }
        }
        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Static

        public static void Start(string[] args, Action<HostBuilderContext, IServiceCollection> configureDelegate = null)
        {
            using (var knifeHost = new KnifeHost(args, configureDelegate))
            {
                knifeHost.Run();
            }
        }



        #endregion
    }
}
