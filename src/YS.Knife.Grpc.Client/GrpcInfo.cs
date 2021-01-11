using System;
using System.Collections.Generic;

namespace YS.Knife.Grpc.Client
{
    public class GrpcInfo
    {
        public GrpcInfo()
        {
        }
        public GrpcInfo(string baseAddress)
        {
            this.BaseAddress = baseAddress;
        }
        public string BaseAddress { get; set; }
        // public Dictionary<string, string> Headers { get; set; }
    }

    public class GrpcInfo<T> : GrpcInfo
        where T : class
    {

    }
}
