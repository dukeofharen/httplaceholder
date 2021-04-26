using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.Application.Interfaces.Validation;
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
            var results = _modelValidator.ValidateModel(stub);
            return results.Select(r => r.ErrorMessage).ToArray();
        }
    }
}
