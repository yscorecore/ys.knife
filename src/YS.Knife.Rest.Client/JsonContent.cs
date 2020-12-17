using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace YS.Knife.Rest.Client
{
    public class JsonContent : StringContent
    {
        static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        public JsonContent(object body) : base(JsonSerializer.Serialize(body, JsonOptions), Encoding.UTF8, "application/json")

        {

        }
    }
}
