using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HttPlaceholder.Common
{
    public interface IModelValidator
    {
        IEnumerable<ValidationResult> ValidateModel(object model);
    }
}
