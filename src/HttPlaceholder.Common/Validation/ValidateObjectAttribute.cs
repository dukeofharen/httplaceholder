using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HttPlaceholder.Common.Validation
{
    // Source: http://www.technofattie.com/2011/10/05/recursive-validation-using-dataannotations.html
    public class ValidateObjectAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (value is IEnumerable enumerable)
            {
                foreach (var subObject in enumerable)
                {
                    var context = new ValidationContext(subObject, null, null);
                    Validator.TryValidateObject(subObject, context, results, true);
                }
            }
            else
            {
                var context = new ValidationContext(value, null, null);
                Validator.TryValidateObject(value, context, results, true);
            }

            if (results.Count != 0)
            {
                var compositeResults =
                    new CompositeValidationResult(string.Format("Validation for {0} failed!",
                        validationContext.DisplayName));
                results.ForEach(compositeResults.AddResult);

                return compositeResults;
            }

            return ValidationResult.Success;
        }
    }

    public class CompositeValidationResult : ValidationResult
    {
        private readonly List<ValidationResult> _results = new List<ValidationResult>();

        public IEnumerable<ValidationResult> Results => _results;

        public CompositeValidationResult(string errorMessage) : base(errorMessage) { }

        public CompositeValidationResult(string errorMessage, IEnumerable<string> memberNames) : base(errorMessage,
            memberNames)
        {
        }

        protected CompositeValidationResult(ValidationResult validationResult) : base(validationResult) { }

        public void AddResult(ValidationResult validationResult) => _results.Add(validationResult);
    }
}
