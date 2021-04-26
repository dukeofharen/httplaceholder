using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HttPlaceholder.Application.Interfaces.Validation
{
    public interface IModelValidator
    {
        IEnumerable<ValidationResult> ValidateModel(object model);
    }
}
