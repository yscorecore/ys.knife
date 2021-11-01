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
                context.Database.Migrate();
                context.Topics.Add(new Topic
                {
                    Content = "some contnet",
                    Title = "this is title"
                });
                context.SaveChanges();
                var topic = context.Topics.Where(p => p.Title.StartsWith("this")).FirstOrDefault();
                topic.Title = topic.Title.ToUpper();
                context.SaveChanges();
                context.Topics.Remove(topic);
                context.SaveChanges();

            }
            Console.ReadKey();
        }
    }
}
