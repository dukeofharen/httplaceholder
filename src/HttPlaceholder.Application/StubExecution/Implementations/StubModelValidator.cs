using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Interfaces.Validation;
using HttPlaceholder.Common.Validation;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Application.StubExecution.Implementations
{
    internal class StubModelValidator : IStubModelValidator
    {
        private readonly IModelValidator _modelValidator;
        private readonly SettingsModel _settings;

        public StubModelValidator(
            IModelValidator modelValidator,
            IOptions<SettingsModel> options)
        {
            _modelValidator = modelValidator;
            _settings = options.Value;
        }

        public IEnumerable<string> ValidateStubModel(StubModel stub)
        {
            var validationResults = _modelValidator.ValidateModel(stub);
            var result = new List<string>();
            HandleValidationResult(result, validationResults);

            // Validate other settings here.
            var extraDurationMillis = stub?.Response?.ExtraDuration ?? 0;
            var allowedMillis = _settings.Stub.MaximumExtraDurationMillis;
            if (extraDurationMillis > 0 && extraDurationMillis > allowedMillis)
            {
                result.Add($"Value for 'ExtraDuration' cannot be higher than '{allowedMillis}'.");
            }

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
