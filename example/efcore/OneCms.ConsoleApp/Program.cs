using System;
using OneCms.EFCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
namespace OneCms.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var host = new YS.Knife.Hosting.KnifeHost(args))
            {
                var context = host.GetService<CmsContext>();
                context.Database.EnsureDeleted();
                context.Database.Migrate();
                context.Topics.Add(new Topic
                {
                    Content = "some contnet",
                    Title = "this is title"
                });
                context.SaveChanges();
            }
            Console.ReadKey();
        }
    }
}
