using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace YS.Knife.Redis
{
    public class ServiceRegister : IServiceRegister
    {
        public void RegisterServices(IServiceCollection services, IRegisterContext context)
        {
            services.AddSingleton<IConnectionMultiplexer>((sp) =>
            {
                var options = sp.GetService<RedisOptions>();
                return CreateConnection(options);
            });
            services.AddSingleton<IDatabase>(sp =>
            {
                var connection = sp.GetRequiredService<IConnectionMultiplexer>();
                return connection.GetDatabase();
            });
        }

        private ConnectionMultiplexer CreateConnection(RedisOptions options)
        {
            return options.Configuration != null ?
                    ConnectionMultiplexer.Connect(options.Configuration) :
                    ConnectionMultiplexer.Connect(options.ConnectionString);
        }

       
    }
}
