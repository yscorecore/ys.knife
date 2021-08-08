using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using YS.Knife.TestData;

namespace YS.Knife
{

    public class MultiServiceTest
    {
        [Fact]
        public void ShouldGetMultiInstanceWhenMultiDefineKnifeAttribute()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();
            var sp = services.RegisterKnifeServices(configuration).BuildServiceProvider();
            var instances = sp.GetServices<MultiService>();
            instances.Count().Should().Be(4);
        }

        [Fact]
        public void ShouldNotGetInstanceWhenJustDefineKnifeAttributeInParents()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();
            var sp = services.RegisterKnifeServices(configuration).BuildServiceProvider();
            var instances = sp.GetServices<SubClass>();
            instances.Count().Should().Be(0);
        }

        [Fact]
        public void ShouldGetMultiInstanceWhenMultiDefineKnifeAttributeInNestedType()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();
            var sp = services.RegisterKnifeServices(configuration).BuildServiceProvider();
            var instances = sp.GetServices<OutterClass.InnerClass>();
            instances.Count().Should().Be(4);
        }

    }
}
