using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentAssertions;
using Xunit;
namespace YS.Knife.Aop.UnitTest
{


    public class MixedValidationAttributeTest
    {
        [Fact]
        public void ShouldValidateSuccessWhenGivenValidValue()
        {
            var entity = new DataEntity
            {
                Prop1 = "valid"
            };
            var validationContext = new ValidationContext(entity);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(entity, validationContext, validationResults, true);
            validationResults.Should().BeEmpty();
        }
        [Fact]
        public void ShouldHasRequiredRule()
        {
            var entity = new DataEntity
            {
                Prop1 = null
            };
            var validationContext = new ValidationContext(entity);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(entity, validationContext, validationResults, true);
            validationResults.Should().HaveCount(1);
            validationResults.First().ErrorMessage.Should().Be("The Prop1 field is required.");

        }
        [Fact]
        public void ShouldHasRegularExpressionRule()
        {
            var entity = new DataEntity
            {
                Prop1 = "Hello,World"
            };
            var validationContext = new ValidationContext(entity);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(entity, validationContext, validationResults, true);
            validationResults.Should().HaveCount(1);
            validationResults.First().ErrorMessage.Should().Be("The field Prop1 must match the regular expression '^\\w+$'.");
        }
    }

    public class Rule1Attribute : MixedValidationAttribute
    {
        public Rule1Attribute() : base(
            new RequiredAttribute(),
            new RegularExpressionAttribute("^\\w+$"))
        {
        }
    }

    public class DataEntity
    {
        [Rule1]
        public string Prop1 { get; set; }
    }
}
