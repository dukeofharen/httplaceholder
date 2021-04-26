using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using HttPlaceholder.Application.Interfaces.Validation;
using HttPlaceholder.Common.Validation;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.Implementations
{
    internal class StubModelValidator : IStubModelValidator
    {
        private readonly IModelValidator _modelValidator;

        public StubModelValidator(IModelValidator modelValidator)
        {
            _modelValidator = modelValidator;
        }

        public IEnumerable<string> ValidateStubModel(StubModel stub)
        {
            var validationResults = _modelValidator.ValidateModel(stub);
            var result = new List<string>();
            HandleValidationResult(result, validationResults);

            return result;
        }

        private static void HandleValidationResult(ICollection<string> result, IEnumerable<ValidationResult> validationResults)
        {
            foreach (var validationResult in validationResults)
            {
                if (validationResult is CompositeValidationResult compositeValidationResult)
                {
                    HandleValidationResult(result, compositeValidationResult.Results);
                }
                else
                {
                    result.Add(validationResult.ErrorMessage);
                }
            }
        }
    }
}
