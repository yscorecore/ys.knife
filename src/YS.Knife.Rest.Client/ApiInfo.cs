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
        public IEnumerable<KeyValuePair<string, string>> Headers { get; set; }
        public HttpContent Body { get; set; }
        public IEnumerable<KeyValuePair<string, string>> Query { get; set; }
        public IDictionary<string, string> Route { get; set; }
    }
}
