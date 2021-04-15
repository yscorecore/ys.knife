using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace YS.Knife.Aop.UnitTest
{

    [TestClass]
    public class MixedValidationAttributeTest
    {
        [TestMethod]
        public void ShouldValidateSuccessWhenGivenValidValue()
        {
            var entity = new DataEntity
            {
                Prop1 = "valid"
            };
            var validationContext = new ValidationContext(entity);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(entity, validationContext, validationResults, true);
            Assert.AreEqual(0, validationResults.Count);
        }
        [TestMethod]
        public void ShouldHasRequiredRule()
        {
            var entity = new DataEntity
            {
                Prop1 = null
            };
            var validationContext = new ValidationContext(entity);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(entity, validationContext, validationResults, true);
            Assert.AreEqual(1, validationResults.Count);
            Assert.AreEqual("The Prop1 field is required.", validationResults.First().ErrorMessage);
        }
        [TestMethod]
        public void ShouldHasRegularExpressionRule()
        {
            var entity = new DataEntity
            {
                Prop1 = "Hello,World"
            };
            var validationContext = new ValidationContext(entity);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(entity, validationContext, validationResults, true);
            Assert.AreEqual(1, validationResults.Count);
            Assert.AreEqual("The field Prop1 must match the regular expression '^\\w+$'.", validationResults.First().ErrorMessage);
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
