using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace System.ComponentModel.DataAnnotations
{
    public class ValidateObjectAttribute : ValidationAttribute
    {
        public ValidateObjectAttribute()
        {

        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var results = ValidComplexObject(value, validationContext);

            if (results.Count != 0)
            {
                var compositeResults = new CompositeValidationResult(string.Format(CultureInfo.InvariantCulture, "{0} validate failed!", validationContext?.MemberName));
                results.ForEach(compositeResults.AddResult);

                return compositeResults;
            }

            return ValidationResult.Success;
        }

        private List<ValidationResult> ValidComplexObject(object value, ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            if (value is IDictionary dictionary)
            {
                foreach (var val in dictionary.Values)
                {
                    if (val == null) continue;
                    var context = new ValidationContext(val, null, null);
                    Validator.TryValidateObject(val, context, results, true);
                }
            }
            if (value is IEnumerable enumerable)
            {
                foreach (var val in enumerable)
                {
                    if (val == null) continue;
                    var context = new ValidationContext(val, null, null);
                    Validator.TryValidateObject(val, context, results, true);
                }
            }
            if (value != null)
            {
                var context = new ValidationContext(value, null, null);
                Validator.TryValidateObject(value, context, results, true);
            }
            return results;
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
