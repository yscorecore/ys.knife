using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace YS.Knife.Rest.Client
{
    public class ApiInfo
    {
        public HttpMethod Method { get; set; }
        public string Path { get; set; }

        public List<ApiArgument> Arguments { get; set; }

    }
}
