using System;
using System.Collections.Generic;

namespace HttPlaceholder.Application.Exceptions
{
    public class ValidationException : Exception
    {
        public IEnumerable<string> ValidationErrors { get; }

        public ValidationException(IEnumerable<string> validationErrors)
        {
            ValidationErrors = validationErrors;
        }

        public override string Message => $"Validation failed:\n{string.Join("\n", ValidationErrors)}";
    }
}
