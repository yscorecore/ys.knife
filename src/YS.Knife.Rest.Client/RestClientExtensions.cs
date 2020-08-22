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
        public static Task SendHttp(this RestClient restClient, HttpMethod method, string path, params ApiArgument[] args)
        {
            _ = restClient ?? throw new ArgumentNullException(nameof(restClient));
            return restClient.SendHttp(new ApiInfo
            {
                Method = method,
                Path = path,
                Arguments = args.ToList()
            });
        }
        public static Task<T> SendHttp<T>(this RestClient restClient, HttpMethod method, string path, params ApiArgument[] args)
        {
            _ = restClient ?? throw new ArgumentNullException(nameof(restClient));
            return restClient.SendHttp<T>(new ApiInfo
            {
                Method = method,
                Path = path,
                Arguments = args.ToList()
            });
        }





        public static Task<T> Get<T>(this RestClient restClient, string path, object queryData = null, IDictionary<string, string> headers = null)
        {
            List<ApiArgument> allArguments = new List<ApiArgument>();
            var queryDic = ObjectToMap(queryData);
            if (queryDic != null)
            {
                allArguments.AddRange(queryDic.Select(kv => new ApiArgument(ArgumentSource.Query, kv.Key, kv.Value)));
            }
            if (headers != null)
            {
                allArguments.AddRange(headers.Select(kv => new ApiArgument(ArgumentSource.Header, kv.Key, kv.Value)));
            }

            return restClient.SendHttp<T>(HttpMethod.Get, path, allArguments.ToArray());
        }

        public static Task Post(this RestClient restClient, string path, params ApiArgument[] args)
        {
            return restClient.SendHttp(HttpMethod.Post, path, args);
        }

        public static Task PostJson(this RestClient restClient, string path, object data, IDictionary<string, string> headers = null)
        {
            List<ApiArgument> allArguments = new List<ApiArgument>();
            allArguments.Add(new ApiArgument(ArgumentSource.BodyJson, nameof(data), data));
            if (headers != null)
            {
                allArguments.AddRange(headers.Select(kv => new ApiArgument(ArgumentSource.Header, kv.Key, kv.Value)));
            }
            return restClient.Post(path, allArguments.ToArray());
        }
        public static Task PostRaw(this RestClient restClient, string path, string data, IDictionary<string, string> headers = null)
        {
            List<ApiArgument> allArguments = new List<ApiArgument>();
            allArguments.Add(new ApiArgument(ArgumentSource.BodyRaw, nameof(data), data));
            if (headers != null)
            {
                allArguments.AddRange(headers.Select(kv => new ApiArgument(ArgumentSource.Header, kv.Key, kv.Value)));
            }
            return restClient.Post(path, allArguments.ToArray());
        }



        public static Task<T> Post<T>(this RestClient restClient, string path, params ApiArgument[] args)
        {
            return restClient.SendHttp<T>(HttpMethod.Post, path, args);
        }




        public static Task Put(this RestClient restClient, string path, params ApiArgument[] args)
        {
            return restClient.SendHttp(HttpMethod.Put, path, args);
        }
        public static Task<T> Put<T>(this RestClient restClient, string path, params ApiArgument[] args)
        {
            return restClient.SendHttp<T>(HttpMethod.Put, path, args);
        }


        public static Task Delete(this RestClient restClient, string path, params ApiArgument[] args)
        {
            return restClient.SendHttp(HttpMethod.Delete, path, args);
        }
        public static Task<T> Delete<T>(this RestClient restClient, string path, params ApiArgument[] args)
        {
            return restClient.SendHttp<T>(HttpMethod.Delete, path, args);
        }

        private static IDictionary<string, object> ObjectToMap(object obj)
        {
            if (obj == null) return null;
            if (obj is IDictionary<string, object>)
            {
                return obj as IDictionary<string, object>;
            }
            return obj.GetType().GetProperties().Where(p => p.CanRead).ToDictionary(p => p.Name, p => p.GetValue(obj));
        }
    }
}
