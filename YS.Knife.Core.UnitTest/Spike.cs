using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife
{
    [TestClass]
    public class Spike
    {
        [TestMethod]
        public void MyTestMethod()
        {
            var services = new ServiceCollection();
            services.AddTransient(typeof(Spike));
            services.AddScoped(typeof(Spike));
            var provider = services.BuildServiceProvider();
            var items = provider.GetServices(typeof(MutilServiceTest));
            var scope = provider.CreateScope().ServiceProvider;
            Console.WriteLine("==========================");
            Console.WriteLine("Begin scope 1");
            Console.WriteLine("First Item hash:{0:X}", scope.GetService<Spike>().GetHashCode());
            Console.WriteLine("Second Item hash:{0:X}", scope.GetService<Spike>().GetHashCode());
            Console.WriteLine("Third Item hash:{0:X}", scope.GetService<Spike>().GetHashCode());
            
            Console.WriteLine("First List hash:{0:X}", scope.GetService<IEnumerable<Spike>>().GetHashCode());
            Console.WriteLine("Second List hash:{0:X}", scope.GetService<IEnumerable<Spike>>().GetHashCode());
            Console.WriteLine("Third List hash:{0:X}", scope.GetService<IEnumerable<Spike>>().GetHashCode());
            var scope2 = provider.CreateScope().ServiceProvider;

            Console.WriteLine("==========================");
            Console.WriteLine("Begin scope 2");
            Console.WriteLine("First Item hash:{0:X}", scope2.GetService<Spike>().GetHashCode());
            Console.WriteLine("Second Item hash:{0:X}", scope2.GetService<Spike>().GetHashCode());
            Console.WriteLine("Third Item hash:{0:X}", scope2.GetService<Spike>().GetHashCode());


            Console.WriteLine("First List hash:{0:X}", scope2.GetService<IEnumerable<Spike>>().GetHashCode());
            Console.WriteLine("Second List hash:{0:X}", scope2.GetService<IEnumerable<Spike>>().GetHashCode());
            Console.WriteLine("Third List hash:{0:X}", scope2.GetService<IEnumerable<Spike>>().GetHashCode());
        }
    }
}
