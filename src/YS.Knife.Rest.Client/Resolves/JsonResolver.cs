using System.Text.Json;

namespace YS.Knife.Rest.Client.Resolves
{
    public class JsonResolver : IEntityResolver
    {
        static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        public T Resolve<T>(string content)
        {
            return JsonSerializer.Deserialize<T>(content, JsonOptions);
        }
    }
}
