using System;
using System.Collections.Generic;
using System.Linq;

namespace YS.Knife.Rest.Client.Resolves
{
    public static class EntityResolver
    {


        public static readonly IDictionary<string, IEntityResolver> Resolvers =
            new Dictionary<string, IEntityResolver>(StringComparer.InvariantCultureIgnoreCase)
            {

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
