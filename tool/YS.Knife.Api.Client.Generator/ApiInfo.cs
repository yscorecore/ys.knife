using System.Collections.Generic;

namespace YS.Knife.Api.Client.Generator
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
