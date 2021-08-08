using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Xunit;
using YS.Knife.Aop;
namespace YS.Knife.Localization
{

    public class StringResourcesTest
    {
        [Fact]
        public void ShouldGetMethodValueFromDefaultTemplate()
        {
            var provider = Utility.BuildProvider();
            var sr = provider.GetRequiredService<I18N>();
            sr.Hello().Should().Be("Hello,World");
        }

        [Fact]
        public void ShouldGetMethodValueFormatIndexValueFromDefaultTemplate()
        {
            var provider = Utility.BuildProvider();
            var sr = provider.GetRequiredService<I18N>();
            var actual = sr.SayHelloWithIndex("zhangsan", 12);
            actual.Should().Be("Hello, I'm zhangsan, I'm 012 years old.");
        }

        [Fact]
        public void ShouldGetMethodValueFormatIndexValueWithDefaultValueFromDefaultTemplate()
        {
            var provider = Utility.BuildProvider();
            var sr = provider.GetRequiredService<I18N>();
            var actual = sr.SayHelloWithIndexAndDefaultValue("zhangsan");
            actual.Should().Be("Hello, I'm zhangsan, I'm 010 years old.");
        }

        [Fact]
        public void ShouldGetMethodValueFormatParamNameValueFromDefaultTemplate()
        {
            var provider = Utility.BuildProvider();
            var sr = provider.GetRequiredService<I18N>();
            var actual = sr.SayHelloWithName(12, "zhangsan");
            actual.Should().Be("Hello, I'm zhangsan, I'm 012 years old.");
        }

        [Fact]
        public void ShouldGetMethodValueFormatParamNameValueAndIndexValueFromDefaultTemplate()
        {
            var provider = Utility.BuildProvider();
            var sr = provider.GetRequiredService<I18N>();
            var actual = sr.SayHelloWithNameAndIndex(12, "zhangsan");
            actual.Should().Be("Hello, I'm zhangsan, I'm 012 years old.");
        }
    }
}
