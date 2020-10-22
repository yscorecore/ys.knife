using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace YS.Knife.Rest.Client
{
    public class EntityDecoderFactory
    {
        private static IEntityDecoder TEXT_DECODER = new TextDecoder();
        private static IEntityDecoder JSON_DECODER = new JsonDecoder();
        private static IEntityDecoder XML_DECODER = new XmlDecoder();
        private static IDictionary<string, IEntityDecoder> decoders = new Dictionary<string, IEntityDecoder>(StringComparer.InvariantCultureIgnoreCase)
        {
            ["text/plain"] = TEXT_DECODER,
            ["text/html"] = TEXT_DECODER,
            ["text/xml"] = XML_DECODER,
            ["application/xml"] = XML_DECODER,
            ["application/json"] = JSON_DECODER
        };
        
        public static IEntityDecoder GetDecoder(string contentType)
        {
            
           
            if (decoders.TryGetValue(contentType, out var decoder))
            {
                return decoder;
            }
            string contentTypes = string.Join(",", decoders.Keys.Select(p => "\"${p}\""));
            throw new NotSupportedException($"Only support content types: {contentTypes}.");
        }
    }
    public interface IEntityDecoder
    {
        T Decode<T>(string content);
    }
    public class JsonDecoder : IEntityDecoder
    {
        public T Decode<T>(string content)
        {
            throw new NotImplementedException();
        }
    }
    public class TextDecoder : IEntityDecoder
    {
        public T Decode<T>(string content)
        {
            throw new NotImplementedException();
        }
    }
    public class XmlDecoder : IEntityDecoder
    {
        public T Decode<T>(string content)
        {
            throw new NotImplementedException();
        }
    }
}