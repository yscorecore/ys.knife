using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using YS.Knife.Localization;
namespace YS.Knife.Aop.CodeExceptions
{

    public class CodeExceptionsTest
    {
        private IAllErrors _allErrors;
        public CodeExceptionsTest()
        {
            var provider = Utility.BuildProvider();
            _allErrors = provider.GetRequiredService<IAllErrors>();
        }


        [Fact]
        public void ShouldGetCodeExceptionWhenNoArgumentAndReturnException()
        {
            var actual = _allErrors.NoArgumentReturnException();
            var expected = new CodeException("100001", "no argument return exception.");

            actual.Should().BeOfType<CodeException>()
                .Which.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ShouldGetCodeExceptionWhenNoArgumentAndReturnApplicationException()
        {
            var actual = _allErrors.NoArgumentReturnApplicationException();
            var expected = new CodeException("100002", "no argument return application exception.");

            actual.Should().BeOfType<CodeException>()
                .Which.Should().BeEquivalentTo(expected);
        }


        [Fact]
        public void ShouldGetCodeExceptionWhenNoArgumentAndReturnCodeException()
        {
            var actual = _allErrors.NoArgumentReturnCodeException();
            var expected = new CodeException("100003", "no argument return code exception.");

            actual.Should().BeOfType<CodeException>()
                .Which.Should().BeEquivalentTo(expected);
        }


        [Fact]

        public void ShouldThrowInvalidOperationExceptionWhenReturnTypeIsArgumentException()
        {
            Action act = () => _allErrors.ReturnArgumentException();
            act.Should().Throw<InvalidOperationException>()
                .WithMessage($"The return type of the method '{nameof(_allErrors.ReturnArgumentException)}' in interface '{typeof(IAllErrors).FullName}' should assignable from '{typeof(CodeException).FullName}'.");
        }


        [Fact]
        public void ShouldGetCodeExceptionWhenWithNameArgument()
        {
            var actual = _allErrors.WithNameArgument("abc");
            var expected = new CodeException("100005", "the argument value is abc.");
            expected.Data["arg"] = "abc";
            actual.Should().BeOfType<CodeException>()
                .Which.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ShouldGetCodeExceptionWhenWithIndexArgument()
        {
            var actual = _allErrors.WithIndexArgument("abc");
            var expected = new CodeException("100006", "the argument value is abc.");
            expected.Data["arg"] = "abc";
            actual.Should().BeOfType<CodeException>()
                .Which.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ShouldGetCodeExceptionWhenWithNameArgumentAndHasDefaultValue()
        {
            var actual = _allErrors.WithNameArgumentAndHasDefaultValue();
            var expected = new CodeException("100007", "the argument value is abc.");
            expected.Data["arg"] = "abc";
            actual.Should().BeOfType<CodeException>()
                .Which.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ShouldGetCodeExceptionWhenWithNameArgumentAndHasFormat()
        {
            var actual = _allErrors.WithNameArgumentAndHasFormat(12);
            var expected = new CodeException("100008", "the argument value is 012.");
            expected.Data["arg"] = 12;
            actual.Should().BeOfType<CodeException>()
                .Which.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ShouldGetCodeExceptionWhenWithNameArgumentAndHasFormatAndHasWidth()
        {
            var actual = _allErrors.WithNameArgumentAndHasFormatAndWidth(12);
            var expected = new CodeException("100009", "the argument value is   012.");
            expected.Data["arg"] = 12;
            actual.Should().BeOfType<CodeException>()
                .Which.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ShouldGetCodeExceptionWhenWithNameArgumentAndHasFormatAndHasNegativeWidth()
        {
            var actual = _allErrors.WithNameArgumentAndHasFormatAndNegativeWidth(12);
            var expected = new CodeException("100010", "the argument value is 012  .");
            expected.Data["arg"] = 12;
            actual.Should().BeOfType<CodeException>()
                .Which.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ShouldGetCodeExceptionWhenMixedNameArgumentAndIndexArgument()
        {
            var actual = _allErrors.MixedNameArgumentAndIndexArgument(12, 13);
            var expected = new CodeException("100011", "first value is 012, second value is 013.");
            expected.Data["arg1"] = 12;
            expected.Data["arg2"] = 13;
            actual.Should().BeOfType<CodeException>()
                .Which.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ShouldGetCodeExceptionWhenMissingNameArgument()
        {
            var actual = _allErrors.MissingNameArgument();
            var expected = new CodeException("100012", "value is [null].");
            actual.Should().BeOfType<CodeException>()
                .Which.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ShouldGetCodeExceptionWhenMissingIndexArgument()
        {
            var actual = _allErrors.MissingIndexArgument();
            var expected = new CodeException("100013", "value is [null].");
            actual.Should().BeOfType<CodeException>()
                .Which.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ShouldGetCodeExceptionWithExceptionWhenUseNameArgument()
        {
            var actual = _allErrors.WithInnerExceptionAndHasNameArgument(new Exception("some error"), "abc");
            var expected = new CodeException("100014", "Value 'abc' Error 'System.Exception: some error'.")
                .WithException(new Exception("some error"))
                .WithData("arg", "abc");
            actual.Should().BeOfType<CodeException>()
                .Which.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ShouldGetCodeExceptionWithExceptionWhenUseIndexArgument()
        {
            var actual = _allErrors.WithInnerExceptionAndHasIndexArgument(new Exception("some error"), "abc");
            var expected = new CodeException("100015", "Value 'abc' Error 'System.Exception: some error'.")
                .WithException(new Exception("some error"))
                .WithData("arg", "abc");
            actual.Should().BeOfType<CodeException>()
                .Which.Should().BeEquivalentTo(expected);
        }
        [Fact]
        public void ShouldGetCodeExceptionWithEmptyStringMessageWhenMessageTemplateIsNull()
        {
            var actual = _allErrors.NullTemplate("abc");
            var expected = new CodeException("100016", string.Empty)
                .WithData("arg", "abc");
            actual.Should().BeOfType<CodeException>()
                .Which.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ShouldReturnNullWhenNoDefineCeAttribute()
        {
            var actual = _allErrors.NoDefineCeAttributeWillReturnNull();
            actual.Should().BeNull();
        }
    }
}
