using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

namespace Knife.WebHosting
{
    public static class Host
    {
        public static void Run(string[] args)
        {
            using (var host = CreateWebHost(args))
            {
                host.Run();
            }
        }
        public static IHost CreateWebHost(string[] args = default)
        {
            return CreateWebHostBuilder(args).Build();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args = default)
        {
            return Knife.Hosting.Host.CreateHostBuilder(args)
                  .ConfigureServices((builder, serviceCollections) =>
                  {
                      var options = builder.Configuration.GetConfigOrNew<WebAppOptions>();
                      IMvcBuilder mvcBuilder = serviceCollections.AddControllers((mvc) =>
                      {

                      });
                      var parts = from p in AppDomain.CurrentDomain.GetAssemblies()
                                  where p.GetName().Name.IsMatchWildcardAnyOne(options.MvcParts, StringComparison.OrdinalIgnoreCase)
                                  select p;
                      foreach (var mvcPart in parts)
                      {
                          mvcBuilder.AddApplicationPart(mvcPart);
                      }
                  })
                  .ConfigureWebHostDefaults(webBuilder =>
                  {
                      webBuilder.UseStartup<Startup>();
                  });


        }
    }
}
