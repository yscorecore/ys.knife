using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace YS.Knife.Grpc.TestServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var host = new YS.Knife.Hosting.KnifeWebHost(args))
            {
                host.Run();
            }
        }


    }
}
