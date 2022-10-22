using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Common;

namespace HttPlaceholder.Infrastructure.Implementations;

internal class ModelValidator : IModelValidator, ISingletonService
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
