using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;

namespace System.ComponentModel.DataAnnotations
{
    public class ValidateObjectAttribute : ValidationAttribute
    {
        public ValidateObjectAttribute()
        {

        }
        public override bool IsValid(object value)
        {
            return base.IsValid(value);
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(value, null, null);
            Validator.TryValidateObject(value, context, results, true);
          
            if (results.Count != 0)
            {
                var compositeResults = new CompositeValidationResult(string.Format(CultureInfo.InvariantCulture, "{0} validate failed!", validationContext.MemberName));
                results.ForEach(compositeResults.AddResult);

                return compositeResults;
            }

            return ValidationResult.Success;
        }

        protected override ValidationResult IsValidDic(object value, ValidationContext validationContext)
        {
            if (value is IDictionary dictionary)
            {
                foreach (var val in dictionary.Values)
                { 
                    
                }
            }
        }
    }

    public class CompositeValidationResult : ValidationResult
    {
        private readonly List<ValidationResult> _results = new List<ValidationResult>();

        public IEnumerable<ValidationResult> Results
        {
            get
            {
                return _results;
            }
        }

        public CompositeValidationResult(string errorMessage) : base(errorMessage) { }
        public CompositeValidationResult(string errorMessage, IEnumerable<string> memberNames) : base(errorMessage, memberNames) { }
        protected CompositeValidationResult(ValidationResult validationResult) : base(validationResult) { }

        public void AddResult(ValidationResult validationResult)
        {
            _results.Add(validationResult);
        }
    }
}
