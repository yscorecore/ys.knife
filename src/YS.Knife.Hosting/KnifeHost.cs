using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
#pragma warning disable CA2214
            this.host = CreateHostBuilder(args ?? Array.Empty<string>()).Build();
#pragma warning restore CA2214
        }

        private void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            var handlers = args.LoadedAssembly.FindInstanceTypesByBaseType<IAssemblyLoadedHander>();
            foreach (var assemblyLoadedHander in handlers)
            {
                var handler = Activator.CreateInstance(assemblyLoadedHander) as IAssemblyLoadedHander;
                handler.AfterAssemblyLoaded();
            }
        }

        private readonly IHost host;

        object IServiceProvider.GetService(Type serviceType)
        {
            return host.Services.GetService(serviceType);
        }


        internal virtual ServiceProviderOptions ServiceProviderOptions
        {
            get
            {
                return new ServiceProviderOptions { ValidateScopes = false, ValidateOnBuild = false };
            }
        }

        protected virtual IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .UseSystemd()
                .UseAopServiceProviderFactory(ServiceProviderOptions)
                .ConfigureLogging(OnConfigureLogging)
                .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
                {
                    this.OnConfigureAppConfiguration(hostBuilderContext, configurationBuilder);
                    this.InjectInternalConfigurations(hostBuilderContext, configurationBuilder);
                })
                .ConfigureServices((builder, serviceCollection) =>
                {
                    serviceCollection.AddAllKnifeServices(builder.Configuration, ShouldFilterType);

                    this.OnConfigureCustomServices(builder, serviceCollection);
                    this.OnConfigureInternalServices(builder, serviceCollection);
                    this.InjectInternalServices(builder, serviceCollection);
                });
        }


        protected virtual void OnConfigureInternalServices(HostBuilderContext builder,
            IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(typeof(KnifeHost), this);
            serviceCollection.AddSingleton(typeof(IServiceCollection), serviceCollection);
        }

        private void OnConfigureCustomServices(HostBuilderContext builder,
            IServiceCollection serviceCollection)
        {
        }

        protected virtual bool ShouldFilterType(Type type)
        {
            return false;
        }

        private void InjectInternalServices(HostBuilderContext builder, IServiceCollection serviceCollection)
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                               BindingFlags.Static;
            var fields = this.GetType().GetFields(bindingFlags)
                .Where(p => Attribute.IsDefined(p, typeof(InjectAttribute)));
            foreach (var field in fields)
            {
                InjectAttribute inject = field.GetCustomAttribute<InjectAttribute>();
                serviceCollection.Add(new ServiceDescriptor(field.FieldType, (sp) => field.GetValue(this),
                    inject.Lifetime));
            }

            // props
            var props = this.GetType().GetProperties(bindingFlags)
                .Where(p => Attribute.IsDefined(p, typeof(InjectAttribute)));
            foreach (var prop in props)
            {
                InjectAttribute inject = prop.GetCustomAttribute<InjectAttribute>();
                serviceCollection.Add(new ServiceDescriptor(prop.PropertyType, (sp) => prop.GetValue(this),
                    inject.Lifetime));
            }
        }

        private void InjectInternalConfigurations(HostBuilderContext _, IConfigurationBuilder configurationBuilder)
        {
            Dictionary<string, string> configurationData = new Dictionary<string, string>();
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                              BindingFlags.Static;
            var fields = this.GetType().GetFields(bindingFlags)
                .Where(p => Attribute.IsDefined(p, typeof(InjectConfigurationAttribute)));
            this.AppendFieldsConfigurationData(fields, configurationData);
            // props
            var props = this.GetType().GetProperties(bindingFlags)
                .Where(p => Attribute.IsDefined(p, typeof(InjectConfigurationAttribute)));
            this.AppendPropertiesConfigurationData(props, configurationData);

            if (configurationData.Count > 0)
            {
                configurationBuilder.AddInMemoryCollection(configurationData);
            }
        }

        private void AppendFieldsConfigurationData(IEnumerable<FieldInfo> fieldInfos, Dictionary<string, string> data)
        {
            foreach (var field in fieldInfos)
            {
                var attr = field.GetCustomAttribute<InjectConfigurationAttribute>();
                AppendConfigurationDataValue(data, attr.ConfigurationKey, field.GetValue(this));
            }
        }

        private void AppendPropertiesConfigurationData(IEnumerable<PropertyInfo> properties,
            Dictionary<string, string> data)
        {
            foreach (var prop in properties)
            {
                var attr = prop.GetCustomAttribute<InjectConfigurationAttribute>();
                AppendConfigurationDataValue(data, attr.ConfigurationKey, prop.GetValue(this));
            }
        }

        private void AppendConfigurationDataValue(Dictionary<string, string> data, string key, object value)
        {
            if (value is IDictionary dic)
            {
                foreach (var subKey in dic.Keys)
                {
                    string childKey = $"{key}:{subKey}";
                    AppendConfigurationDataValue(data, childKey, dic[subKey]);
                }
            }

            data[key] = value.ToString();
        }

        protected virtual void OnConfigureLogging(HostBuilderContext context, ILoggingBuilder loggingBuilder)
        {
        }

        protected virtual void OnConfigureAppConfiguration(HostBuilderContext hostBuilderContext,
            IConfigurationBuilder configurationBuilder)
        {
        }


        public void Run()
        {
            this.PrintDebugInfo();
            var options = this.host.Services.GetService<KnifeOptions>();
            if (string.IsNullOrEmpty(options?.Stage))
            {
                this.host.Run();
            }
            else
            {
                var logger = this.host.Services.GetKnifeLogger();
                logger.LogInformation($"knife host is running in stage mode, the stage name is '{options?.Stage}'.");
                this.host.RunStage(options.Stage);
            }
        }

        private void PrintDebugInfo()
        {
            var logger = this.host.Services.GetKnifeLogger();
            var hostEnv = this.host.Services.GetService<IHostEnvironment>();
            if (hostEnv.IsDevelopment())
            {
                // only in development mode
                LogConfigurationDebugInfo(logger);
                LogDependencyInjectionServicesInfo(logger);
            }
        }

        private void LogConfigurationDebugInfo(ILogger logger)
        {
            var configRoot = this.host.Services.GetRequiredService<IConfiguration>() as IConfigurationRoot;
            if (configRoot == null) return;
            StringBuilder content = new StringBuilder();
            content.AppendLine("===================Configuration Values===============");
            content.AppendLine(configRoot.GetDebugView());
            content.AppendLine("======================================================");
            logger.LogDebug(content.ToString());
        }

        private void LogDependencyInjectionServicesInfo(ILogger logger)
        {
            var serviceCollection = this.host.Services.GetRequiredService<IServiceCollection>();
            if (serviceCollection == null) return;
            StringBuilder content = new StringBuilder();
            content.AppendLine("====================Injection Services================");
            content.AppendLine(GetDebugView(serviceCollection));
            content.AppendLine("======================================================");
            logger.LogDebug(content.ToString());
        }

        private string GetDebugView(IServiceCollection serviceCollection)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"a total of {serviceCollection.Count} services were injected.");
            var sequence = NewSequence();
            foreach (var sp in serviceCollection)
            {
                if (sp.ImplementationType != null)
                {
                    stringBuilder.AppendLine(
                        $"[{sequence():000}] [{sp.Lifetime}] {sp.ServiceType} from type {sp.ImplementationType}");
                }
                else if (sp.ImplementationInstance != null)
                {
                    stringBuilder.AppendLine(
                        $"[{sequence():000}] [{sp.Lifetime}] {sp.ServiceType} from instance");
                }
                else if (sp.ImplementationFactory != null)
                {
                    stringBuilder.AppendLine(
                        $"[{sequence():000}] [{sp.Lifetime}] {sp.ServiceType} from factory {sp.ImplementationFactory.Method}");
                }
            }

            return stringBuilder.ToString();

            Func<int> NewSequence()
            {
                int value = 1;
                return () => value++;
            }
        }

        public async Task RunAsync(CancellationToken token = default)
        {
            var options = this.host.Services.GetService<KnifeOptions>();

            if (string.IsNullOrEmpty(options?.Stage))
            {
                await this.host.RunAsync(token);
            }
            else
            {
                var logger = this.host.Services.GetKnifeLogger();
                logger.LogInformation($"knife host is running in stage mode, the stage name is '{options?.Stage}'.");
                this.host.RunStage(options.Stage, token);
            }
        }

        #region IDisposable Support

        private bool disposedValue; // 要检测冗余调用

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
