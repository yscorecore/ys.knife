using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
                .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
                {
                    this.OnConfigureAppConfiguration(hostBuilderContext, configurationBuilder);
                    this.InjectInternalConfigurations(hostBuilderContext, configurationBuilder);
                })
                .ConfigureServices((builder, serviceCollection) =>
                {
                    serviceCollection.AddAllKnifeServices(builder.Configuration, ShouldFilterType);

                    this.OnConfigureCustomService(builder, serviceCollection);
                    this.InjectInternalServices(builder, serviceCollection);
                });
        }

        protected virtual void OnConfigureCustomService(HostBuilderContext builder, IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(typeof(KnifeHost), this);
        }
        protected virtual bool ShouldFilterType(Type type)
        {
            return false;
        }
        private void InjectInternalServices(HostBuilderContext builder, IServiceCollection serviceCollection)
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
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
        private void InjectInternalConfigurations(HostBuilderContext _, IConfigurationBuilder configurationBuilder)
        {
            Dictionary<string, string> configurationDatas = new Dictionary<string, string>();
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            var fields = this.GetType().GetFields(bindingFlags).Where(p => Attribute.IsDefined(p, typeof(InjectConfigurationAttribute)));
            this.AppendFieldsConfigurationData(fields, configurationDatas);
            // props
            var props = this.GetType().GetProperties(bindingFlags).Where(p => Attribute.IsDefined(p, typeof(InjectConfigurationAttribute)));
            this.AppendPropertiesConfigurationData(props, configurationDatas);

            if (configurationDatas.Count > 0)
            {
                configurationBuilder.AddInMemoryCollection(configurationDatas);
            }

        }
        private void AppendFieldsConfigurationData(IEnumerable<FieldInfo> fieldInfos, Dictionary<string, string> datas)
        {
            foreach (var field in fieldInfos)
            {
                var attr = field.GetCustomAttribute<InjectConfigurationAttribute>();
                AppendConfigurationDataValue(datas, attr.ConfigurationKey, field.GetValue(this));
            }
        }
        private void AppendPropertiesConfigurationData(IEnumerable<PropertyInfo> properties, Dictionary<string, string> datas)
        {
            foreach (var prop in properties)
            {
                var attr = prop.GetCustomAttribute<InjectConfigurationAttribute>();
                AppendConfigurationDataValue(datas, attr.ConfigurationKey, prop.GetValue(this));
            }
        }
        private void AppendConfigurationDataValue(Dictionary<string, string> datas, string key, object value)
        {
            if (value is null) return;
            if (value is IDictionary dic)
            {
                foreach (var subKey in dic.Keys)
                {
                    string childKey = $"{key}:{subKey}";
                    AppendConfigurationDataValue(datas, childKey, dic[subKey]);
                }
            }
            datas[key] = value.ToString();

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
