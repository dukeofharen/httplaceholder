using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HttPlaceholder.Common;

/// <summary>
/// Describes a class that is used to validate a given model.
/// </summary>
public interface IModelValidator
{
    /// <summary>
    /// Validates a given model.
    /// </summary>
    /// <param name="model">The model to validate.</param>
    /// <returns>A list of <see cref="ValidationResult"/>.</returns>
    IEnumerable<ValidationResult> ValidateModel(object model);
}
