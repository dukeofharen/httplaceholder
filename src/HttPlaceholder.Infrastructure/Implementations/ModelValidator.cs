using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HttPlaceholder.Common;

namespace HttPlaceholder.Infrastructure.Implementations;

/// <inheritdoc />
internal class ModelValidator : IModelValidator
{
    /// <inheritdoc />
    public IEnumerable<ValidationResult> ValidateModel(object model)
    {
        var context = new ValidationContext(model, null, null);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, context, results, true);
        return results;
    }
}
