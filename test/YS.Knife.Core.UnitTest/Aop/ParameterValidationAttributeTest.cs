using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
namespace YS.Knife.Aop
{

    public class ParameterValidationAttributeTest
    {
        private IServiceProvider provider;
        public ParameterValidationAttributeTest()
        {
            provider = Utility.BuildProvider();
        }

        [Fact]
        public void ShouldSuccessIfNotDefineParameterValidation()
        {
            var service = provider.GetService<INoDefineParameterValidation>();
            service.AnyString(null);
            service.AnyString("abc");
        }

        [Fact]
        public void ShouldThrowIfDefineParameterValidationInInterfaceMethod()
        {
            var service = provider.GetService<INoDefineParameterValidation>();
            var action = new Action(() =>
            {
                service.MustBeUrlWillEffectiveBecauseDefineParameterValidationInInterface("abc");
            });


            var exception = action.Should().ThrowExactly<ValidationException>().Which;


            exception.ValidationResult.MemberNames.Should().Contain("url");
            exception.Value.Should().Be("abc");
            exception.ValidationAttribute.GetType().Should().Be(typeof(UrlAttribute));
        }

        [Fact]
        public void ShouldThrowIfDefineParameterValidationInImplementationMethod()
        {
            var service = provider.GetService<INoDefineParameterValidation>();
            var action = new Action(() =>
            {
                service.MustBeUrlWillEffectiveBecauseDefineParameterValidationInImplementation("abc");
            });

            var exception = action.Should().ThrowExactly<ValidationException>().Which;

            exception.ValidationResult.MemberNames.Should().Contain("url");
            exception.Value.Should().Be("abc");
            exception.ValidationAttribute.GetType().Should().Be(typeof(UrlAttribute));
        }

        [Fact]
        public void ShouldThrowIfDefineParameterValidationInInterfaceType()
        {
            var service = provider.GetService<IDefineParameterValidation>();
            var action = new Action(() =>
            {
                service.MustbeEmailAddress("abc");
            });
            var exception = action.Should().ThrowExactly<ValidationException>().Which;
            exception.ValidationResult.MemberNames.Should().Contain("emailAddress");
            exception.Value.Should().Be("abc");
            exception.ValidationAttribute.GetType().Should().Be(typeof(EmailAddressAttribute));
        }

        [Fact]
        public void ShouldThrowIfImplementationMethodDefineRequiredAttribute()
        {
            var service = provider.GetService<IDefineParameterValidation>();
            var action = new Action(() =>
            {
                service.MustbeEmailAddress(null);
            });
            var exception = action.Should().ThrowExactly<ValidationException>().Which;
            exception.ValidationResult.MemberNames.Should().Contain("emailAddress");
            exception.Value.Should().Be(null);
            exception.ValidationAttribute.GetType().Should().Be(typeof(RequiredAttribute));
        }

        [Fact]
        public void ShouldThrowIfImplementationMethodDefineRequiredAttributeAndGivenComplexType()
        {
            var service = provider.GetService<IDefineParameterValidation>();
            var action = new Action(() =>
            {
                service.ValidateComplexClass(null);
            });
            var exception = action.Should().ThrowExactly<ValidationException>().Which;
            exception.ValidationResult.MemberNames.Should().Contain("cpmplexObject");
            exception.Value.Should().Be(null);
            exception.ValidationAttribute.GetType().Should().Be(typeof(RequiredAttribute));
        }

        [Fact]
        public void ShouldThrowIfComplexTypeValidateFail()
        {
            var service = provider.GetService<IDefineParameterValidation>();
            var action = new Action(() =>
            {
                service.ValidateComplexClass(new ComplexClass { Value = -1 });
            });
            var exception = action.Should().ThrowExactly<ValidationException>().Which;
            exception.ValidationResult.MemberNames.Should().Contain("Value");
            exception.Value.Should().Be(-1);
            exception.ValidationAttribute.GetType().Should().Be(typeof(RangeAttribute));
        }

        //[Fact]
        // public void ShouldThrowIfComplexNestedTypeValidateFail()
        // {
        //     var service = this.GetService<IDefineParameterValidation>();
        //     var exception = Assert.ThrowsException<ValidationException>(() =>
        //     {
        //         service.ValidateComplexClass(new ComplexClass { Value = 5, NestedProp = new NestedClass { NestedValue = "too long......" } });
        //     });
        //     Assert.IsTrue(exception.ValidationResult.MemberNames.Contains("NestedValue"));
        //      exception.Value.Should().Be(-1);
        //      exception.ValidationAttribute.GetType().Should().Be(typeof(StringLengthAttribute));
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
