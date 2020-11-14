using System;
using System.Collections.Generic;
using System.Linq;

namespace YS.Knife.Rest.Client.Resolves
{
    public static class EntityResolver
    {
        private static readonly IEntityResolver _textResolver = new TextResolver();
        private static readonly IEntityResolver _jsonResolver = new JsonResolver();
        private static readonly IEntityResolver _xmlResolver = new XmlResolver();

        public static readonly IDictionary<string, IEntityResolver> Resolvers =
            new Dictionary<string, IEntityResolver>(StringComparer.InvariantCultureIgnoreCase)
            {
                ["text/plain"] = _textResolver,
                ["text/html"] = _textResolver,
                ["text/xml"] = _xmlResolver,
                ["application/xml"] = _xmlResolver,
                ["application/json"] = _jsonResolver
            };

        private static IEntityResolver GetResolver(string contentType)
        {
            if (Resolvers.TryGetValue(contentType, out var resolver))
            {
                return resolver;
            }
            string contentTypes = string.Join(",", Resolvers.Keys.Select(p => "\"${p}\""));
            throw new NotSupportedException($"Only support content types: {contentTypes}.");
        }

        public static T Resolve<T>(string contentType, string content)
        {
            return GetResolver(contentType).Resolve<T>(content);
        }
    }
}
