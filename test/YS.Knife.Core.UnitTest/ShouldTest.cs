using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace YS.Knife
{
    [TestClass]
    public class ShouldTest
    {
        [TestMethod]
        public void ShouldNotThrowExceptionWhenNotNullCheckNotNullValue()
        {
            Should.NotNull("", () => new CodeException());
            Should.NotNull(1, () => new CodeException());
            Should.NotNull(new object(), () => new CodeException());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ShouldThrowExceptionWhenNotNullCheckNullValue()
        {
            Should.NotNull(null, () => new InvalidOperationException());
        }

        [TestMethod]
        public void ShouldNotThrowExceptionWhenNotEmptyCheckNotEmptyString()
        {
            Should.NotEmpty("a", () => new CodeException());
            Should.NotEmpty(" ", () => new CodeException());
            Should.NotEmpty("\t", () => new CodeException());
        }

        [TestMethod]
        [ExpectedException(typeof(CodeException))]
        public void ShouldThrowExceptionWhenNotEmptyCheckEmptyString()
        {
            Should.NotEmpty("", () => new CodeException());
        }

        [TestMethod]
        [ExpectedException(typeof(CodeException))]
        public void ShouldThrowExceptionWhenNotEmptyCheckNullString()
        {
            Should.NotEmpty(null, () => new CodeException());
        }

        [TestMethod]
        public void ShouldNotThrowExceptionWhenNotEmptyCheckNotEmptyCollection()
        {
            Should.NotEmpty(new[] { 1 }, () => new CodeException());

        }

        [TestMethod]
        [ExpectedException(typeof(CodeException))]
        public void ShouldNotThrowExceptionWhenNotEmptyCheckEmptyCollection()
        {
            Should.NotEmpty(new string[0], () => new CodeException());

        }

        [TestMethod]
        [ExpectedException(typeof(CodeException))]
        public void ShouldNotThrowExceptionWhenNotEmptyCheckNullCollection()
        {
            IEnumerable<string> enumerable = null;
            Should.NotEmpty(enumerable, () => new CodeException());
        }



        [TestMethod]
        public void ShouldNotThrowExceptionWhenNotBlankCheckNotBlankString()
        {
            Should.NotBlank("a", () => new CodeException());
            Should.NotBlank(" a ", () => new CodeException());
        }

        [TestMethod]
        [ExpectedException(typeof(CodeException))]
        public void ShouldThrowExceptionWhenNotBlankCheckEmptyString()
        {
            Should.NotBlank("", () => new CodeException());
        }

        [TestMethod]
        [ExpectedException(typeof(CodeException))]
        public void ShouldThrowExceptionWhenNotBlankCheckNullString()
        {
            Should.NotBlank(null, () => new CodeException());
        }
        [TestMethod]
        [ExpectedException(typeof(CodeException))]
        public void ShouldThrowExceptionWhenNotBlankCheckBlankString()
        {
            Should.NotBlank(" \t ", () => new CodeException());
        }

        [TestMethod]
        public void ShouldNotThrowExceptionWhenBeTrueCheckTrueValue()
        {
            Should.BeTrue(true, () => new CodeException());
        }

        [TestMethod]
        [ExpectedException(typeof(CodeException))]
        public void ShouldThrowExceptionWhenBeTrueCheckFalseValue()
        {
            Should.BeTrue(false, () => new CodeException());
        }

        [TestMethod]
        public void ShouldNotThrowExceptionWhenBeFalseCheckFalseValue()
        {
            Should.BeFalse(false, () => new CodeException());
        }

        [TestMethod]
        [ExpectedException(typeof(CodeException))]
        public void ShouldThrowExceptionWhenBeFalseCheckTrueValue()
        {
            Should.BeFalse(true, () => new CodeException());
        }
    }
}
