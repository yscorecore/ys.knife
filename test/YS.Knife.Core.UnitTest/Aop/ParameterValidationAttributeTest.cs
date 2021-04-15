using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace YS.Knife.Aop
{
    [TestClass]
    public class ParameterValidationAttributeTest
    {
        private IServiceProvider provider;
        [TestInitialize]
        public void Setup()
        {
            provider = Utility.BuildProvider();
        }

        [TestMethod]
        public void ShouldSuccessIfNotDefineParameterValidation()
        {
            var service = provider.GetService<INoDefineParameterValidation>();
            service.AnyString(null);
            service.AnyString("abc");
        }

        [TestMethod]
        public void ShouldThrowIfDefineParameterValidationInInterfaceMethod()
        {
            var service = provider.GetService<INoDefineParameterValidation>();
            var exception = Assert.ThrowsException<ValidationException>(() =>
            {
                service.MustBeUrlWillEffectiveBecauseDefineParameterValidationInInterface("abc");
            });
            Assert.IsTrue(exception.ValidationResult.MemberNames.Contains("url"));
            Assert.AreEqual("abc", exception.Value);
            Assert.AreEqual(typeof(UrlAttribute), exception.ValidationAttribute.GetType());
        }

        [TestMethod]
        public void ShouldThrowIfDefineParameterValidationInImplementationMethod()
        {
            var service = provider.GetService<INoDefineParameterValidation>();
            var exception = Assert.ThrowsException<ValidationException>(() =>
            {
                service.MustBeUrlWillEffectiveBecauseDefineParameterValidationInImplementation("abc");
            });
            Assert.IsTrue(exception.ValidationResult.MemberNames.Contains("url"));
            Assert.AreEqual("abc", exception.Value);
            Assert.AreEqual(typeof(UrlAttribute), exception.ValidationAttribute.GetType());
        }

        [TestMethod]
        public void ShouldThrowIfDefineParameterValidationInInterfaceType()
        {
            var service = provider.GetService<IDefineParameterValidation>();
            var exception = Assert.ThrowsException<ValidationException>(() =>
            {
                service.MustbeEmailAddress("abc");
            });
            Assert.IsTrue(exception.ValidationResult.MemberNames.Contains("emailAddress"));
            Assert.AreEqual("abc", exception.Value);
            Assert.AreEqual(typeof(EmailAddressAttribute), exception.ValidationAttribute.GetType());
        }

        [TestMethod]
        public void ShouldThrowIfImplementationMethodDefineRequiredAttribute()
        {
            var service = provider.GetService<IDefineParameterValidation>();
            var exception = Assert.ThrowsException<ValidationException>(() =>
            {
                service.MustbeEmailAddress(null);
            });
            Assert.IsTrue(exception.ValidationResult.MemberNames.Contains("emailAddress"));
            Assert.AreEqual(null, exception.Value);
            Assert.AreEqual(typeof(RequiredAttribute), exception.ValidationAttribute.GetType());
        }

        [TestMethod]
        public void ShouldThrowIfImplementationMethodDefineRequiredAttributeAndGivenComplexType()
        {
            var service = provider.GetService<IDefineParameterValidation>();
            var exception = Assert.ThrowsException<ValidationException>(() =>
            {
                service.ValidateComplexClass(null);
            });
            Assert.IsTrue(exception.ValidationResult.MemberNames.Contains("cpmplexObject"));
            Assert.AreEqual(null, exception.Value);
            Assert.AreEqual(typeof(RequiredAttribute), exception.ValidationAttribute.GetType());
        }

        [TestMethod]
        public void ShouldThrowIfComplexTypeValidateFail()
        {
            var service = provider.GetService<IDefineParameterValidation>();
            var exception = Assert.ThrowsException<ValidationException>(() =>
            {
                service.ValidateComplexClass(new ComplexClass { Value = -1 });
            });
            Assert.IsTrue(exception.ValidationResult.MemberNames.Contains("Value"));
            Assert.AreEqual(-1, exception.Value);
            Assert.AreEqual(typeof(RangeAttribute), exception.ValidationAttribute.GetType());
        }

        // [TestMethod]
        // public void ShouldThrowIfComplexNestedTypeValidateFail()
        // {
        //     var service = this.GetService<IDefineParameterValidation>();
        //     var exception = Assert.ThrowsException<ValidationException>(() =>
        //     {
        //         service.ValidateComplexClass(new ComplexClass { Value = 5, NestedProp = new NestedClass { NestedValue = "too long......" } });
        //     });
        //     Assert.IsTrue(exception.ValidationResult.MemberNames.Contains("NestedValue"));
        //     Assert.AreEqual(-1, exception.Value);
        //     Assert.AreEqual(typeof(StringLengthAttribute), exception.ValidationAttribute.GetType());
        // }
    }

    public interface INoDefineParameterValidation
    {
        void MustBeUrlWillNotEffective([Url] string url);

        [ParameterValidationAttribute]
        void MustBeUrlWillEffectiveBecauseDefineParameterValidationInInterface([Url] string url);

        void MustBeUrlWillEffectiveBecauseDefineParameterValidationInImplementation([Url] string url);
        void AnyString([Url] string url);

    }

    [ParameterValidationAttribute]
    public interface IDefineParameterValidation
    {
        void MustbeEmailAddress([EmailAddress] string email);

        void ValidateComplexClass(ComplexClass cpmplexObject);

    }



    [Service(typeof(IDefineParameterValidation))]
    [Service(typeof(INoDefineParameterValidation))]
    public class Servic1 : IDefineParameterValidation, INoDefineParameterValidation
    {
        public void AnyString([Url] string url)
        {
            AllValidationWillNotEffective(url);
        }

        [ParameterValidationAttribute]
        public void MustBeUrlWillEffectiveBecauseDefineParameterValidationInImplementation([Url] string url)
        {

        }

        public void MustbeEmailAddress([Required] string emailAddress)
        {
        }

        public void MustBeUrlWillEffectiveBecauseDefineParameterValidationInInterface([Url] string url)
        {
        }

        public void MustBeUrlWillNotEffective([Url] string url)
        {
        }

        public void ValidateComplexClass([Required] ComplexClass cpmplexObject)
        {
        }

        public void AllValidationWillNotEffective([Url][Required] string url)
        {

        }

    }

    public class ComplexClass
    {
        [Range(1, 10)]
        public int Value { get; set; }
        public NestedClass NestedProp { get; set; }
    }
    public class NestedClass
    {
        [StringLength(5)]
        public string NestedValue { get; set; }
    }
}
