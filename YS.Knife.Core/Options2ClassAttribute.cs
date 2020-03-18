using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
namespace YS.Knife
{
    public class Options2ClassAttribute : GenericKnifeAttribute
    {
        public string ConfigKey { get; set; }

        protected override IGenericRegisteProxy OnGetGenericProxy(KnifeRegisteContext context)
        {
            var type = typeof(OptionsRegisteProxy<>).MakeGenericType(context.DeclaredType);
            return Activator.CreateInstance(type, ConfigKey) as IGenericRegisteProxy;
        }

        private class OptionsRegisteProxy<T> : IGenericRegisteProxy
           where T : class
        {
            public OptionsRegisteProxy(string configKey)
            {
                this.ConfigKey = configKey;
            }
            private string ConfigKey { get; set; }
            public void RegisteService(KnifeRegisteContext context)
            {
                var optionsConfiguration = context.Configuration.GetOptionsConfiguration<T>(ConfigKey);
                context.Services.AddOptions<T>().Bind(optionsConfiguration).ValidateDataAnnotations();
            }
        }
    }
}
