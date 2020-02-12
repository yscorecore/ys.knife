using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using CommonHost = Knife.Hosting.Host;
namespace Knife.WebHosting
{
    public static class Host
    {
        public static void Run(string[] args)
        {
            CommonHost.Run(args, CreateWebHost);
        }
        public static IHost CreateWebHost(string[] args = default)
        {
            return CreateWebHostBuilder(args).Build();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args = default)
        {
            return CommonHost.CreateHostBuilder(args)
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
