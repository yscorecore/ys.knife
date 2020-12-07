using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YS.Knife.Stages;
namespace YS.Knife.Hosting
{

    public class KnifeHost : IDisposable, IServiceProvider
    {
        public KnifeHost() : this(Array.Empty<string>())
        {
        }
        public KnifeHost(IDictionary<string, object> args)
            : this(args?.Select(p => $"/{p.Key}={p.Value}").ToArray())
        {
        }
        public KnifeHost(string[] args)
        {
#pragma warning disable CA2214 
            this.host = CreateHostBuilder(args ?? Array.Empty<string>()).Build();
#pragma warning restore CA2214
        }

        private readonly IHost host;

        public object GetService(Type serviceType)
        {
            return host.Services.GetService(serviceType);
        }
        public T GetService<T>()
        {
            return ServiceProviderServiceExtensions.GetService<T>(this);
        }

        protected virtual IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseAopServiceProviderFactory()
                .ConfigureLogging(OnConfigureLogging)
                .ConfigureAppConfiguration(OnConfigureAppConfiguration)
                .ConfigureServices((builder, serviceCollection) =>
                {
                    serviceCollection.AddAllKnifeServices(builder.Configuration, ShouldFilterType);
                    this.OnConfigureCustomService(builder, serviceCollection);
                });
        }

        protected virtual void OnConfigureCustomService(HostBuilderContext builder, IServiceCollection serviceCollection)
        {
            this.InjectServices(builder, serviceCollection);
            serviceCollection.AddSingleton(typeof(KnifeHost), this);
        }
        protected virtual bool ShouldFilterType(Type type)
        {
            return false;
        }
        private void InjectServices(HostBuilderContext builder, IServiceCollection serviceCollection)
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var fields = this.GetType().GetFields(bindingFlags).Where(p => Attribute.IsDefined(p, typeof(InjectAttribute)));
            foreach (var field in fields)
            {
                InjectAttribute inject = field.GetCustomAttribute<InjectAttribute>();
                serviceCollection.Add(new ServiceDescriptor(field.FieldType, (sp) => field.GetValue(this), inject.Lifetime));
            }
            // props
            var props = this.GetType().GetProperties(bindingFlags).Where(p => Attribute.IsDefined(p, typeof(InjectAttribute)));
            foreach (var prop in props)
            {
                InjectAttribute inject = prop.GetCustomAttribute<InjectAttribute>();
                serviceCollection.Add(new ServiceDescriptor(prop.PropertyType, (sp) => prop.GetValue(this), inject.Lifetime));
            }
        }
        protected virtual void OnConfigureLogging(HostBuilderContext context, ILoggingBuilder loggingBuilder)
        {
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

    }
}
