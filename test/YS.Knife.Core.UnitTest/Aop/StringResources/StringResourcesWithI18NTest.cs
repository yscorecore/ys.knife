using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Knife.Aop.StringResources
{
    [TestClass]
    public class StringResourcesWithI18NTest
    {
        private I18NWithResx _i18N;
        [TestInitialize]
        public void Setup()
        {
            var provider = Utility.BuildProvider();
            _i18N = provider.GetRequiredService<I18NWithResx>();
        }

        [TestMethod]
        public void ShouldGetCodeTemplateMessageWhenNoConfigKeyInI18NResx()
        {
            var actual = _i18N.NotConfigKeyInI18NResx();
            var expected = "code template first";

            actual.Should().Be(expected);
        }

        [TestMethod]
        public void ShouldGetEmptyMessageWhenConfigEmptyTemplateInI18NResx()
        {
            var actual = _i18N.ConfigEmptyTemplateInI18NResx();
            var expected = string.Empty;

            actual.Should().Be(expected);
        }

        [TestMethod]
        public void ShouldGetConfigValueWhenConfigSomeValueInI18NResx()
        {
            var actual = _i18N.ConfigSomeValueInI18NResx();
            var expected = "abc";

            actual.Should().Be(expected);
        }

        [TestMethod]
        public void ShouldGetFormatMessageWhenConfigNameTemplateValueInI18NResx()
        {
            var actual = _i18N.ConfigNameTemplateValueInI18NResx(1);
            var expected = "value is   001.";

            actual.Should().Be(expected);
        }

        [TestMethod]
        public void ShouldGetFormatMessageWhenConfigIndexTemplateValueInI18NResx()
        {
            var actual = _i18N.ConfigIndexTemplateValueInI18NResx(1);
            var expected = "value is   001.";

            actual.Should().Be(expected);
        }
    }
}
