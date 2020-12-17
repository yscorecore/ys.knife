using StackExchange.Redis;

namespace YS.Knife.Redis
{
    [Options("Redis")]
    public class RedisOptions
    {
        public string ConnectionString { get; set; } = "localhost";
        public ConfigurationOptions Configuration { get; set; }
    }
}
