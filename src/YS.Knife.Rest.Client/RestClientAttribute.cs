using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.Rest.Client
{

    public class RestClientAttribute : KnifeAttribute
    {
        public string DefaultBaseAddress { get; set; }
        public Type InjectType { get; set; }
        public Type[] MessageHandlerTypes { get; }
        public RestClientAttribute(params Type[] messageHandlerTypes) : base(typeof(RestClient))
        {
            this.MessageHandlerTypes = messageHandlerTypes;
            this.CheckMessageHandlerTypes(messageHandlerTypes);
        }
        public RestClientAttribute(string defaultBaseAddress, params Type[] messageHandlerTypes) : base(typeof(RestClient))
        {
            this.DefaultBaseAddress = defaultBaseAddress;
            this.MessageHandlerTypes = messageHandlerTypes;
            this.CheckMessageHandlerTypes(messageHandlerTypes);
        }
        private void CheckMessageHandlerTypes(Type[] messageHandlerTypes)
        {
            if (messageHandlerTypes != null)
            {
                foreach (var messageHandler in messageHandlerTypes)
                {
                    if (messageHandler == null) continue;
                    if (!messageHandler.IsClass)
                    {
                        throw new ArgumentException($"The type \"{messageHandler.FullName}\" should be a class type.");
                    }
                    if (messageHandler.IsAbstract)
                    {
                        throw new ArgumentException($"The type \"{messageHandler.FullName}\" should not be an abstract type.");
                    }
                    if (!typeof(DelegatingHandler).IsAssignableFrom(messageHandler))
                    {
                        throw new ArgumentException($"The type \"{messageHandler.FullName}\" should not be a sub type from \"{typeof(DelegatingHandler).FullName}\".");
                    }
                }
            }
        }
        public override void RegisterService(IServiceCollection services, IRegisterContext context, Type declareType)
        {
            _ = declareType ?? throw new ArgumentNullException(nameof(declareType));

            var injectType = this.InjectType ?? DeduceInjectType(declareType);
            if (injectType != null)
            {
                services.AddTransient(injectType, (sp) => sp.GetService(declareType));
            }
            var method = typeof(RestClientAttribute).GetMethod(nameof(RegisterHttpClientAndRestInfo), BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(declareType);
            method.Invoke(this, new object[] { services, this.MessageHandlerTypes });
        }
        private void RegisterHttpClientAndRestInfo<T>(IServiceCollection services, Type[] handlers)
           where T : class
        {
            var httpClientBuilder = services.AddHttpClient<T>();
            AddMessageHandlers(httpClientBuilder, handlers);
            services.AddSingleton<RestInfo<T>>((sp) =>
            {
                var options = sp.GetRequiredService<ApiServicesOptions>();
                if (options.Services.TryGetValue(typeof(T).FullName, out ServiceOptions serviceOptions))
                {
                    return new RestInfo<T>
                    {
                        BaseAddress = serviceOptions.BaseAddress,
                        Headers = serviceOptions.Headers,
                    };
                }
                return new RestInfo<T>()
                {
                };
            });
        }
        private void AddMessageHandlers(IHttpClientBuilder builder, Type[] handlerTypes)
        {
            if (handlerTypes != null)
            {
                Array.ForEach(handlerTypes, handlerType => AddMessageHandler(builder, handlerType));
            }
        }
        private void AddMessageHandler(IHttpClientBuilder builder, Type handlerType)
        {
            if (handlerType != null)
            {
                builder.AddHttpMessageHandler((sp) => (DelegatingHandler)ActivatorUtilities.CreateInstance(sp, handlerType));
            }
        }

        private Type DeduceInjectType(Type serviceType)
        {
            var allInterfaces = serviceType.GetInterfaces();
            if (allInterfaces.Length > 1)
            {
                throw new InvalidOperationException($"Can not deduce the inject type from current type '{serviceType.FullName}'.");
            }
            else if (allInterfaces.Length == 0)
            {
                return serviceType;
            }
            else
            {
                return allInterfaces.FirstOrDefault();
            }


        }
    }
}
