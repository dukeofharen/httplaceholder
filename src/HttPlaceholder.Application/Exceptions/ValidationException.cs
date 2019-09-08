using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using FluentValidation.Results;

namespace HttPlaceholder.Application.Exceptions
{
    [Serializable]
    public class ValidationException : Exception
    {
        public ValidationException(IEnumerable<string> failures)
        {
            Failures = failures;
        }

        public ValidationException(List<ValidationFailure> failures)
        {
            Failures = failures
                .Select(f => f.ErrorMessage)
                .ToArray();
        }

        protected ValidationException(SerializationInfo serializationInfo, StreamingContext streamingContext) :
            base(serializationInfo, streamingContext)
        {
        }

        public IEnumerable<string> Failures { get; }
    }
}
