using System.Text.Json;

namespace YS.Knife.Rest.Client.Resolves
{
    public class JsonResolver : IEntityResolver
    {
        public T Resolve<T>(string content)
        {
            return Json.DeSerialize<T>(content);
        }
    }
}
