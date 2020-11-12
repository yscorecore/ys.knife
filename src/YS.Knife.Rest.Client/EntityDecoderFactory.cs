using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.Rest.Client
{
    public class EntityDecoderFactory
    {
        private static IEntityResolver _textResolver = new TextResolver();
        private static IEntityResolver _jsonResolver = new JsonResolver();
        private static IEntityResolver _xmlResolver = new XmlResolver();
        public static IDictionary<string, IEntityResolver> decoders = new Dictionary<string, IEntityResolver>(StringComparer.InvariantCultureIgnoreCase)
        {
            ["text/plain"] = _textResolver,
            ["text/html"] = _textResolver,
            ["text/xml"] = _xmlResolver,
            ["application/xml"] = _xmlResolver,
            ["application/json"] = _jsonResolver
        };

        public static IEntityResolver GetDecoder(string contentType)
        {
            if (decoders.TryGetValue(contentType, out var decoder))
            {
                return decoder;
            }
            string contentTypes = string.Join(",", decoders.Keys.Select(p => "\"${p}\""));
            throw new NotSupportedException($"Only support content types: {contentTypes}.");
        }

        public static T Decode<T>(string contentType, string content)
        {
            var decoder = GetDecoder(contentType);
            return decoder.Decode<T>(content);
        }

    }
    public interface IEntityResolver
    {
        T Decode<T>(string content);
    }
    public class JsonResolver : IEntityResolver
    {
        static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        public T Decode<T>(string content)
        {
            return JsonSerializer.Deserialize<T>(content, JsonOptions);
        }
    }
    public class TextResolver : IEntityResolver
    {
        public T Decode<T>(string content)
        {
            if (typeof(T) == typeof(string))
            {
                return (T)(object)content;
            }
            throw new NotSupportedException();
        }
    }
    public class XmlResolver : IEntityResolver
    {
        public T Decode<T>(string content)
        {
            var ser = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using (StringReader sr = new StringReader(content))
            {
                return (T)ser.Deserialize(sr);
            }
        }
    }
}
