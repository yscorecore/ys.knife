using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Rest.Api.Client.Generator
{
    public class ApiInfo
    {
        public string ControllerName { get; set; }
        public string ControllerRouter { get; set; }
        public string Method { get; set; }
        public string ActionRouter { get; set; }

        public IDictionary<string, string> Arguments { get; set; }
    }
}
