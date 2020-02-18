using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Knife.Rest.Client
{
    public class RestApiInfo
    {
        public HttpMethod Method { get; set; }
        public string Path { get; set; }

        public List<RestArgument> Arguments { get; set; }

    }
}
