using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
namespace YS.Knife
{
    public class Options2ClassAttribute : KnifeAttribute
    {
        public string ConfigKey { get; set; }

        public override void RegisteService(KnifeRegisteContext registerContext)
        {
            var method = this.GetType().GetMethod(nameof(RegisteOptions), BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(registerContext.DeclaredType);
            method.Invoke(this, new object[] { registerContext.Services, registerContext.Configuration });
        }
        private void RegisteOptions<T>(IServiceCollection services, IConfiguration configuration)
            where T : class
        {
            var optionsConfiguration = configuration.GetOptionsConfiguration<T>(ConfigKey);
            services.AddOptions<T>().Bind(optionsConfiguration).ValidateDataAnnotations();
        }
        private class OptionsRegisteProxy<T>
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
