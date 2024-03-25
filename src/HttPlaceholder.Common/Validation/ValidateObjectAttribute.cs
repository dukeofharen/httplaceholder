using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HttPlaceholder.Common.Validation;

// Source: http://www.technofattie.com/2011/10/05/recursive-validation-using-dataannotations.html
/// <summary>
///     A validation attribute that can be used to validate properties other than primitive types (so also arrays, lists,
///     classes etc.).
/// </summary>
public class ValidateObjectAttribute : ValidationAttribute
{
    /// <inheritdoc />
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();
        switch (value)
        {
            case null:
                return ValidationResult.Success;
            case IEnumerable enumerable:
            {
                foreach (var subObject in enumerable)
                {
                    var context = new ValidationContext(subObject, null, null);
                    Validator.TryValidateObject(subObject, context, results, true);
                }

                break;
            }
            default:
            {
                var context = new ValidationContext(value, null, null);
                Validator.TryValidateObject(value, context, results, true);
                break;
            }
        }

        if (results.Count == 0)
        {
            return ValidationResult.Success;
        }

        var compositeResults =
            new CompositeValidationResult($"Validation for {validationContext.DisplayName} failed!");
        results.ForEach(compositeResults.AddResult);

        return compositeResults;
    }
}

/// <summary>
///     A class that contains multiple validation results.
/// </summary>
public class CompositeValidationResult : ValidationResult
{
    private readonly List<ValidationResult> _results = [];

    /// <summary>
    ///     Constructs a <see cref="CompositeValidationResult" />.
    /// </summary>
    /// <param name="errorMessage"></param>
    public CompositeValidationResult(string errorMessage) : base(errorMessage) { }

    /// <summary>
    ///     Gets the validation results.
    /// </summary>
    public IEnumerable<ValidationResult> Results => _results;

    /// <summary>
    ///     Adds a validation result to the list.
    /// </summary>
    /// <param name="validationResult"></param>
    public void AddResult(ValidationResult validationResult) => _results.Add(validationResult);
}
