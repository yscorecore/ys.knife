using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace YS.Knife.Grpc.TestServer
{
    public class Program : YS.Knife.Hosting.KnifeWebHost
    {
        public Program(string[] args) : base(args)
        {

        }
        public static void Main(string[] args)
        {
            new Program(args).Run();
        }
        protected override void OnConfigureWebHostBuilder(IWebHostBuilder webBuilder)
        {
            //webBuilder.UseKestrel(kestrelServerOptions =>
            //{
            //    kestrelServerOptions.ListenLocalhost(5000, o => o.Protocols =
            //        HttpProtocols.Http2);
            //});
        }
    }
}
