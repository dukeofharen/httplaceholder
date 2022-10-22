using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.Utilities;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Common.Validation;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Application.StubExecution.Implementations;

internal class StubModelValidator : IStubModelValidator, ISingletonService
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

    /// <inheritdoc />
    public IEnumerable<string> ValidateStubModel(StubModel stub)
    {
        var validationResults = _modelValidator.ValidateModel(stub);
        var result = new List<string>();
        HandleValidationResult(result, validationResults);

        ValidateExtraDuration(stub, result);
        ValidateScenarioVariables(stub, result);
        ValidateResponseBody(stub, result);
        return result;
    }

    private void ValidateExtraDuration(StubModel stub, List<string> result)
    {
        const string errorTemplate = "Value for '{0}' cannot be higher than '{1}'.";
        var extraDuration = stub?.Response?.ExtraDuration;
        var allowedMillis = _settings.Stub?.MaximumExtraDurationMillis;
        var parsedDuration = ConversionUtilities.ParseInteger(extraDuration);
        if (parsedDuration.HasValue)
        {
            if (parsedDuration.Value > 0 && parsedDuration.Value > allowedMillis)
            {
                result.Add(string.Format(errorTemplate, "ExtraDuration", allowedMillis));
            }
        }
        else if (extraDuration != null)
        {
            var extraDurationModel = ConversionUtilities.Convert<StubExtraDurationModel>(extraDuration);
            if (extraDurationModel?.Min > 0 && extraDurationModel.Min > allowedMillis)
            {
                result.Add(string.Format(errorTemplate, "ExtraDuration.Min", allowedMillis));
            }

            if (extraDurationModel?.Max > 0 && extraDurationModel.Max > allowedMillis)
            {
                result.Add(string.Format(errorTemplate, "ExtraDuration.Max", allowedMillis));
            }

            if (extraDurationModel is {Min: { }} && extraDurationModel.Max.HasValue &&
                extraDurationModel.Min > extraDurationModel.Max)
            {
                result.Add("ExtraDuration.Min should be lower than or equal to ExtraDuration.Max.");
            }
        }
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

    private static void ValidateResponseBody(StubModel stub, ICollection<string> validationErrors)
    {
        var response = stub?.Response;
        if (response == null)
        {
            return;
        }

        var count = StringHelper.CountNumberOfNonWhitespaceStrings(response.Text, response.Json, response.Xml,
            response.Html, response.Base64, response.File);
        switch (count)
        {
            case 1 when response.StatusCode == (int)HttpStatusCode.NoContent:
                validationErrors.Add("When HTTP status code is 204, no response body can be set.");
                break;
            case > 1:
                validationErrors.Add(
                    "Only one of the response body fields (text, json, xml, html, base64, file) can be set");
                break;
        }
    }
}
