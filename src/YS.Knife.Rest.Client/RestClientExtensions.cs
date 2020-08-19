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
        public static Task Get(this RestClient restClient, string path, params ApiArgument[] args)
        {
            return restClient.SendHttp(HttpMethod.Get, path, args);
        }
        public static Task<T> Get<T>(this RestClient restClient, string path, params ApiArgument[] args)
        {
            return restClient.SendHttp<T>(HttpMethod.Get, path, args);
        }
        public static Task Post(this RestClient restClient, string path, params ApiArgument[] args)
        {
            return restClient.SendHttp(HttpMethod.Post, path, args);
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

    }
}
