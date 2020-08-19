using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Rest.Client
{
    public class RestInfo
    {
        public string BaseAddress { get; set; }
        public TimeSpan Timeout { get; set; }
        public long MaxResponseContentBufferSize { get; set; }
    }
}
