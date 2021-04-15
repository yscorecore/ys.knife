using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace YS.Knife.Aop
{
    public class StringResourcesAttribute : DynamicProxyAttribute
    {
        public StringResourcesAttribute() : base(ServiceLifetime.Singleton)
        {
        }

        //public string ResxFilePath { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class SrAttribute : BaseAopAttribute
    {
        public SrAttribute(string key, string defaultValue)
        {
            this.Key = key;
            this.Value = defaultValue;
        }

        public string Key { get; }

        public string Value { get; }

        public override Task Invoke(AspectContext context, AspectDelegate next)
        {
            var localizer = GetLocalizerInstance(context);
            var resourceKey = string.IsNullOrEmpty(Key) ? context.ServiceMethod.Name : Key;
            var localizedString = localizer?.GetString(resourceKey);
            var template = localizedString.ResourceNotFound ? this.Value : localizedString.Value;
            context.ReturnValue = FormatTemplate(template, context);
            return context.Break();
        }

        //private static LocalCache<Type, string> resourceKeyCache = new LocalCache<Type, string>();
        private IStringLocalizer GetLocalizerInstance(AspectContext context)
        {
            // var resxFilePath = resourceKeyCache.Get(context.ServiceMethod.DeclaringType,
            //     type => type.GetCustomAttribute<StringResourcesAttribute>()?.ResxFilePath);
            //if (string.IsNullOrEmpty(resxFilePath))
            {
                var type = typeof(IStringLocalizer<>).MakeGenericType(context.ServiceMethod.DeclaringType);
                return context.ServiceProvider.GetRequiredService(type) as IStringLocalizer;
            }
            // else
            // {
            //     var assemblyName = new AssemblyName(context.ServiceMethod.DeclaringType.GetTypeInfo().Assembly.FullName);
            //     var name = assemblyName.Name;
            //     var factory = context.ServiceProvider.GetRequiredService<IStringLocalizerFactory>();
            //     return factory.Create(resxFilePath, assemblyName.Name);
            // }

            return null;
        }

        private string FormatTemplate(string template, AspectContext context)
        {
            var formatter = ValuesFormatter.FromText(template ?? string.Empty);
            var kwArgs = context.ServiceMethod.GetParameters()
                .Zip(context.Parameters, (pInfo, val) => new KeyValuePair<string, object>(pInfo.Name, val))
                .ToList();
            return formatter.Format(kwArgs);
        }
    }


}
