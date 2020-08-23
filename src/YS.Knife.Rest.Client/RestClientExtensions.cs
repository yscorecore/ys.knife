using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Rest.Client
{
    public static class RestClientExtensions
    {
        public static Task SendHttp(this RestClient restClient, HttpMethod method, string path, object route, object header, object query, HttpContent body)
        {
            _ = restClient ?? throw new ArgumentNullException(nameof(restClient));
            return restClient.SendHttp(new ApiInfo
            {
                Method = method,
                Path = path,
                Body = body,
                Route = ObjectUtils.ObjectToStringDictionary(route, true),
                Query = ObjectUtils.ObjectToStringKeyValuePairs(query),
                Headers = ObjectUtils.ObjectToStringKeyValuePairs(header),
            });
        }
        public static Task<T> SendHttp<T>(this RestClient restClient, HttpMethod method, string path, object route, object header, object query, HttpContent body)
        {
            _ = restClient ?? throw new ArgumentNullException(nameof(restClient));
            return restClient.SendHttp<T>(new ApiInfo
            {
                Method = method,
                Path = path,
                Body = body,
                Route = ObjectUtils.ObjectToStringDictionary(route, true),
                Query = ObjectUtils.ObjectToStringKeyValuePairs(query),
                Headers = ObjectUtils.ObjectToStringKeyValuePairs(header),
            });
        }
        #region GET
        public static Task<T> SendGet<T>(this RestClient restClient, string path, object route, object query, object header)
        {
            return restClient.SendHttp<T>(HttpMethod.Get, path, route: route, header: header, query: query, body: null);
        }
        public static Task<T> Get<T>(this RestClient restClient, string path, object query = null, object header = null)
        {
            return restClient.SendGet<T>(path, null, query, header);
        }
        #endregion

        #region POST
        public static Task SendPost(this RestClient restClient, string path, object route, object query, object header, HttpContent body)
        {
            return restClient.SendHttp(HttpMethod.Post, path, route: route, header: header, query: query, body: body);
        }
        public static Task<T> SendPost<T>(this RestClient restClient, string path, object route, object query, object header, HttpContent body)
        {
            return restClient.SendHttp<T>(HttpMethod.Post, path, route: route, header: header, query: query, body: body);
        }


        public static Task PostJson(this RestClient restClient, string path, object body, object header = null)
        {
            var bodyContent = NewJsonContentBody(body);
            return restClient.SendPost(path, route: null, query: null, header: header, body: bodyContent);
        }
        public static Task<T> PostJson<T>(this RestClient restClient, string path, object body, object header = null)
        {
            var bodyContent = NewJsonContentBody(body);
            return restClient.SendPost<T>(path, route: null, query: null, header: header, body: bodyContent);
        }
        public static Task PostUrlEncodeForm(this RestClient restClient, string path, object body, object header = null)
        {
            var bodyContent = NewUrlEncodedBody(body);
            return restClient.SendPost(path, route: null, query: null, header: header, body: bodyContent);
        }
        public static Task<T> PostUrlEncodeForm<T>(this RestClient restClient, string path, object body, object header = null)
        {
            var bodyContent = NewUrlEncodedBody(body);
            return restClient.SendPost<T>(path, route: null, query: null, header: header, body: bodyContent);
        }
        #endregion






        public static JsonContent NewJsonContentBody(object body)
        {
            if (body is JsonContent)
            {
                return body as JsonContent;
            }
            return new JsonContent(body);
        }
        public static FormUrlEncodedContent NewUrlEncodedBody(object body)
        {
            if (body is FormUrlEncodedContent)
            {
                return body as FormUrlEncodedContent;
            }
            return new FormUrlEncodedContent(ObjectUtils.ObjectToStringKeyValuePairs(body));
        }










    }
}
