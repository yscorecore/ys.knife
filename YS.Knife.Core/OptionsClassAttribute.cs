using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
namespace YS.Knife
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class OptionsClassAttribute : KnifeAttribute
    {
        public OptionsClassAttribute(string configKey = default)
        {
            this.ConfigKey = configKey;
        }
        public string ConfigKey { get; set; }

        public override void RegisteService(KnifeRegisteContext context)
        {
            var method = this.GetType().GetMethod(nameof(RegisteOptions), BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(context.DeclaredType);
            method.Invoke(this, new object[] { context.Services, context.Configuration });
        }
        private void RegisteOptions<T>(IServiceCollection services, IConfiguration configuration)
            where T : class
        {
            var optionsConfiguration = configuration.GetOptionsConfiguration<T>(ConfigKey);
            services.AddOptions<T>().Bind(optionsConfiguration).ValidateDataAnnotations();
        }
    }
}
