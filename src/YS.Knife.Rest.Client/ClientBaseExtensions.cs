using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Rest.Client
{
    public static class ClientBaseExtensions
    {
        public static Task SendHttp(this ClientBase clientBase, HttpMethod method, string path, params RestArgument[] args)
        {
            _ = clientBase ?? throw new ArgumentNullException(nameof(clientBase));
            return clientBase.SendHttp(new RestApiInfo
            {
                Method = method,
                Path = path,
                Arguments = args.ToList()
            });
        }
        public static Task<T> SendHttp<T>(this ClientBase clientBase, HttpMethod method, string path, params RestArgument[] args)
        {
            _ = clientBase ?? throw new ArgumentNullException(nameof(clientBase));
            return clientBase.SendHttp<T>(new RestApiInfo
            {
                Method = method,
                Path = path,
                Arguments = args.ToList()
            });
        }
    }
}
