using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;


namespace YS.Knife
{

    public class ShouldTest
    {
        [Fact]
        public void ShouldNotThrowExceptionWhenNotNullCheckNotNullValue()
        {
            Should.NotNull("", () => new CodeException());
            Should.NotNull(1, () => new CodeException());
            Should.NotNull(new object(), () => new CodeException());
        }

        [Fact]
        public void ShouldThrowExceptionWhenNotNullCheckNullValue()
        {
            var action = new Action(() => Should.NotNull(null, () => new InvalidOperationException()));
            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ShouldNotThrowExceptionWhenNotEmptyCheckNotEmptyString()
        {
            Should.NotEmpty("a", () => new CodeException());
            Should.NotEmpty(" ", () => new CodeException());
            Should.NotEmpty("\t", () => new CodeException());
        }

        [Fact]
        public void ShouldThrowExceptionWhenNotEmptyCheckEmptyString()
        {
            var action = new Action(() => Should.NotEmpty("", () => new CodeException()));
            action.Should().Throw<CodeException>();
        }

        [Fact]
        public void ShouldThrowExceptionWhenNotEmptyCheckNullString()
        {
            var action = new Action(() => Should.NotEmpty(null, () => new CodeException()));
            action.Should().Throw<CodeException>();
        }

        [Fact]
        public void ShouldNotThrowExceptionWhenNotEmptyCheckNotEmptyCollection()
        {
            Should.NotEmpty(new[] { 1 }, () => new CodeException());

        }

        [Fact]
        public void ShouldNotThrowExceptionWhenNotEmptyCheckEmptyCollection()
        {
            var action = new Action(() => Should.NotEmpty(new string[0], () => new CodeException()));
            action.Should().Throw<CodeException>();
        }

        [Fact]
        public void ShouldNotThrowExceptionWhenNotEmptyCheckNullCollection()
        {
            IEnumerable<string> enumerable = null;
            var action = new Action(() => Should.NotEmpty(enumerable, () => new CodeException()));
            action.Should().Throw<CodeException>();
        }



        [Fact]
        public void ShouldNotThrowExceptionWhenNotBlankCheckNotBlankString()
        {
            Should.NotBlank("a", () => new CodeException());
            Should.NotBlank(" a ", () => new CodeException());
        }

        [Fact]
        public void ShouldThrowExceptionWhenNotBlankCheckEmptyString()
        {
            var action = new Action(() => Should.NotBlank("", () => new CodeException()));
            action.Should().Throw<CodeException>();
        }

        [Fact]
        public void ShouldThrowExceptionWhenNotBlankCheckNullString()
        {
            var action = new Action(() => Should.NotBlank(null, () => new CodeException()));
            action.Should().Throw<CodeException>();
        }
        [Fact]
        public void ShouldThrowExceptionWhenNotBlankCheckBlankString()
        {
            var action = new Action(() => Should.NotBlank(" \t ", () => new CodeException()));
            action.Should().Throw<CodeException>();
        }

        [Fact]
        public void ShouldNotThrowExceptionWhenBeTrueCheckTrueValue()
        {
            Should.BeTrue(true, () => new CodeException());
        }

        [Fact]
        public void ShouldThrowExceptionWhenBeTrueCheckFalseValue()
        {
            var action = new Action(() => Should.BeTrue(false, () => new CodeException()));
            action.Should().Throw<CodeException>();
        }

        [Fact]
        public void ShouldNotThrowExceptionWhenBeFalseCheckFalseValue()
        {
            Should.BeFalse(false, () => new CodeException());
        }

        [Fact]
        public void ShouldThrowExceptionWhenBeFalseCheckTrueValue()
        {
            var action = new Action(() => Should.BeFalse(true, () => new CodeException()));
            action.Should().Throw<CodeException>();
        }
    }
}
