using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace YS.Knife.Aop.StringResources
{

    public class StringResourcesWithI18NTest
    {
        private I18NWithResx _i18N;
        public StringResourcesWithI18NTest()
        {
            var provider = Utility.BuildProvider();
            _i18N = provider.GetRequiredService<I18NWithResx>();
        }

        [Fact]
        public void ShouldGetCodeTemplateMessageWhenNoConfigKeyInI18NResx()
        {
            var actual = _i18N.NotConfigKeyInI18NResx();
            var expected = "code template first";

            actual.Should().Be(expected);
        }

        [Fact]
        public void ShouldGetEmptyMessageWhenConfigEmptyTemplateInI18NResx()
        {
            var actual = _i18N.ConfigEmptyTemplateInI18NResx();
            var expected = string.Empty;

            actual.Should().Be(expected);
        }

        [Fact]
        public void ShouldGetConfigValueWhenConfigSomeValueInI18NResx()
        {
            var actual = _i18N.ConfigSomeValueInI18NResx();
            var expected = "abc";

            actual.Should().Be(expected);
        }

        [Fact]
        public void ShouldGetFormatMessageWhenConfigNameTemplateValueInI18NResx()
        {
            var actual = _i18N.ConfigNameTemplateValueInI18NResx(1);
            var expected = "value is   001.";

            actual.Should().Be(expected);
        }

        [Fact]
        public void ShouldGetFormatMessageWhenConfigIndexTemplateValueInI18NResx()
        {
            var actual = _i18N.ConfigIndexTemplateValueInI18NResx(1);
            var expected = "value is   001.";

            actual.Should().Be(expected);
        }
    }
}
