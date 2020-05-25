using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Reflection;
using YS.Knife.Options;

namespace YS.Knife
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class OptionsClassAttribute : KnifeAttribute
    {
        public OptionsClassAttribute(string configKey = default) : base(null)
        {
            this.ConfigKey = configKey;
        }
        public string ConfigKey { get; set; }

        public bool EnableFunctionHandler { get; set; } = true;

        public override void RegisteService(IServiceCollection services, IRegisteContext context, Type declareType)
        {
            var method = typeof(OptionsClassAttribute).GetMethod(nameof(RegisteOptions), BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(declareType);
            method.Invoke(this, new object[] { services, context?.Configuration });
        }
        private void RegisteOptions<T>(IServiceCollection services, IConfiguration configuration)
            where T : class, new()
        {
            var optionsConfiguration = configuration.GetOptionsConfiguration<T>(ConfigKey);
            services.AddOptions<T>().Bind(optionsConfiguration).ValidateDataAnnotations();
            if (EnableFunctionHandler)
            {
                services.AddSingleton<IPostConfigureOptions<T>, PostFunctionConfigure<T>>();
            }
        }
    }
}
