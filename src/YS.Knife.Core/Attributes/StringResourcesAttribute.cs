using System;
using System.Linq;
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
    }

    [AttributeUsage(AttributeTargets.Method| AttributeTargets.Property, AllowMultiple=false, Inherited=false)]
    public class SRAttribute : BaseAopAttribute
    {
        public SRAttribute(string key,string defaultValue)
        {
            this.Key=key;
            this.Value=defaultValue;
        }
        public string Key { get; set; }
        
        public string Value {get;set;}

        public override Task Invoke(AspectContext context, AspectDelegate next)
        {
            var type = typeof(IStringLocalizer<>).MakeGenericType(context.ServiceMethod.DeclaringType);
            var localizer =  context.ServiceProvider.GetRequiredService(type) as IStringLocalizer;
            var resourceKey = string.IsNullOrEmpty(Key)? context.ServiceMethod.Name:Key;
            var localizedString = localizer.GetString(resourceKey);
            var template = localizedString.ResourceNotFound? this.Value: localizedString.Value;
            var result = FormatTemplate(template, context);
            context.ReturnValue = FormatTemplate(template, context);
            return context.Break();
        }
    

        private string FormatTemplate(string template, AspectContext context) {
            return template;
        }

    }

}