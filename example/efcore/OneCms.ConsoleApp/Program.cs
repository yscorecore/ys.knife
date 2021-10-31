using System;
using System.Linq;
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
                context.Database.EnsureCreated();
                context.Topics.Add(new Topic
                {
                    Content = "some contnet",
                    Title = "this is title"
                });
                context.SaveChanges();
                var query = context.Topics.Where(p => p.Title != null);
                var topic1 = query.ToList();
                //topic1.Title = topic1.Title.ToUpper();
                //context.SaveChanges();

            }
            Console.ReadKey();
        }
    }
}
