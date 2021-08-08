﻿using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace YS.Knife.Aop.CodeExceptions
{

    public class CodeExceptionsWithI18NTest
    {
        private IAllErrorWithResource _allErrors;

        public CodeExceptionsWithI18NTest()
        {
            var provider = Utility.BuildProvider();
            _allErrors = provider.GetRequiredService<IAllErrorWithResource>();
        }

        [Fact]
        public void ShouldGetCodeTemplateMessageWhenNoConfigKeyInI18NResx()
        {
            var actual = _allErrors.NotConfigKeyInI18NResx();
            var expected = new CodeException("200001", "code template first");

            actual.Should().BeOfType<CodeException>()
                .Which.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ShouldGetEmptyMessageWhenConfigEmptyTemplateInI18NResx()
        {
            var actual = _allErrors.ConfigEmptyTemplateInI18NResx();
            var expected = new CodeException("200002", string.Empty);

            actual.Should().BeOfType<CodeException>()
                .Which.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ShouldGetConfigValueWhenConfigSomeValueInI18NResx()
        {
            var actual = _allErrors.ConfigSomeValueInI18NResx();
            var expected = new CodeException("200003", "abc");

            actual.Should().BeOfType<CodeException>()
                .Which.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ShouldGetFormatMessageWhenConfigNameTemplateValueInI18NResx()
        {
            var actual = _allErrors.ConfigNameTemplateValueInI18NResx(1);
            var expected = new CodeException("200004", "value is   001.")
                .WithData("val", 1);

            actual.Should().BeOfType<CodeException>()
                .Which.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ShouldGetFormatMessageWhenConfigIndexTemplateValueInI18NResx()
        {
            var actual = _allErrors.ConfigIndexTemplateValueInI18NResx(1);
            var expected = new CodeException("200005", "value is   001.")
                .WithData("val", 1);

            actual.Should().BeOfType<CodeException>()
                .Which.Should().BeEquivalentTo(expected);
        }
    }
}
