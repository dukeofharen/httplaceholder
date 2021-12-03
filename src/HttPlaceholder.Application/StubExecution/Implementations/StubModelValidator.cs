using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Common;
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

            // Validate extra duration.
            var extraDurationMillis = stub?.Response?.ExtraDuration ?? 0;
            var allowedMillis = _settings.Stub?.MaximumExtraDurationMillis;
            if (extraDurationMillis > 0 && extraDurationMillis > allowedMillis)
            {
                result.Add($"Value for 'ExtraDuration' cannot be higher than '{allowedMillis}'.");
            }

            // Validate scenario variables.
            ValidateScenarioVariables(stub, result);

            return result;
        }

        private static void HandleValidationResult(ICollection<string> result,
            IEnumerable<ValidationResult> validationResults)
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

        private static void ValidateScenarioVariables(StubModel stub, ICollection<string> validationErrors)
        {
            var scenarioConditions = stub?.Conditions?.Scenario ?? new StubConditionScenarioModel();
            var minHits = scenarioConditions.MinHits;
            var maxHits = scenarioConditions.MaxHits;
            var exactHits = scenarioConditions.ExactHits;
            var scenarioState = scenarioConditions.ScenarioState;
            if (minHits.HasValue && minHits == maxHits)
            {
                validationErrors.Add("minHits and maxHits can not be equal.");
            }

            if (maxHits < minHits)
            {
                validationErrors.Add("maxHits can not be lower than minHits.");
            }

            if (exactHits.HasValue && (minHits.HasValue || maxHits.HasValue))
            {
                validationErrors.Add("exactHits can not be set if minHits and maxHits are set.");
            }

            var scenarioResponse = stub?.Response?.Scenario ?? new StubResponseScenarioModel();
            var setScenarioState = scenarioResponse.SetScenarioState;
            var clearState = scenarioResponse.ClearState;
            if (!string.IsNullOrWhiteSpace(setScenarioState) && clearState == true)
            {
                validationErrors.Add("setScenarioState and clearState can not both be set at the same time.");
            }

            var scenario = stub?.Scenario ?? string.Empty;
            if (string.IsNullOrWhiteSpace(scenario) && (minHits.HasValue || maxHits.HasValue || exactHits.HasValue ||
                                                        !string.IsNullOrWhiteSpace(scenarioState) ||
                                                        !string.IsNullOrWhiteSpace(setScenarioState)))
            {
                validationErrors.Add(
                    "Scenario condition checkers and response writers can not be set if no 'scenario' is provided.");
            }
        }
    }
}
