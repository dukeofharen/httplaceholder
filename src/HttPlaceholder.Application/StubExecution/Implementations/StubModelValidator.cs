using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
    private static readonly string[] _illegalHeaders =
    {
        "X-HttPlaceholder-Correlation", "X-HttPlaceholder-ExecutedStub"
    };

    private readonly IModelValidator _modelValidator;
    private readonly IOptionsMonitor<SettingsModel> _options;

    public StubModelValidator(
        IModelValidator modelValidator,
        IOptionsMonitor<SettingsModel> options)
    {
        _modelValidator = modelValidator;
        _options = options;
    }

    /// <inheritdoc />
    public IEnumerable<string> ValidateStubModel(StubModel stub)
    {
        var validationResults = _modelValidator.ValidateModel(stub);
        var result = new List<string>();
        result.AddRange(HandleValidationResult(validationResults));
        result.AddRange(ValidateExtraDuration(stub));
        result.AddRange(ValidateScenarioVariables(stub));
        result.AddRange(ValidateResponseBody(stub));
        result.AddRange(ValidateResponseHeaders(stub));
        return result;
    }

    private IEnumerable<string> ValidateExtraDuration(StubModel stub)
    {
        var result = new List<string>();
        const string errorTemplate = "Value for '{0}' cannot be higher than '{1}'.";
        var extraDuration = stub?.Response?.ExtraDuration;
        var allowedMillis = _options.CurrentValue.Stub?.MaximumExtraDurationMillis;
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

        return result;
    }

    private static IEnumerable<string> HandleValidationResult(IEnumerable<ValidationResult> validationResults)
    {
        var result = new List<string>();
        foreach (var validationResult in validationResults)
        {
            if (validationResult is CompositeValidationResult compositeValidationResult)
            {
                result.AddRange(HandleValidationResult(compositeValidationResult.Results));
            }
            else
            {
                result.Add(validationResult.ErrorMessage);
            }
        }

        return result;
    }

    private static IEnumerable<string> ValidateScenarioVariables(StubModel stub)
    {
        var result = new List<string>();
        var scenarioConditions = stub?.Conditions?.Scenario ?? new StubConditionScenarioModel();
        var minHits = scenarioConditions.MinHits;
        var maxHits = scenarioConditions.MaxHits;
        var exactHits = scenarioConditions.ExactHits;
        var scenarioState = scenarioConditions.ScenarioState;
        if (minHits.HasValue && minHits == maxHits)
        {
            result.Add("minHits and maxHits can not be equal.");
        }

        if (maxHits < minHits)
        {
            result.Add("maxHits can not be lower than minHits.");
        }

        if (exactHits.HasValue && (minHits.HasValue || maxHits.HasValue))
        {
            result.Add("exactHits can not be set if minHits and maxHits are set.");
        }

        var scenarioResponse = stub?.Response?.Scenario ?? new StubResponseScenarioModel();
        var setScenarioState = scenarioResponse.SetScenarioState;
        var clearState = scenarioResponse.ClearState;
        if (!string.IsNullOrWhiteSpace(setScenarioState) && clearState == true)
        {
            result.Add("setScenarioState and clearState can not both be set at the same time.");
        }

        var scenario = stub?.Scenario ?? string.Empty;
        if (string.IsNullOrWhiteSpace(scenario) && (minHits.HasValue || maxHits.HasValue || exactHits.HasValue ||
                                                    !string.IsNullOrWhiteSpace(scenarioState) ||
                                                    !string.IsNullOrWhiteSpace(setScenarioState)))
        {
            result.Add(
                "Scenario condition checkers and response writers can not be set if no 'scenario' is provided.");
        }

        return result;
    }

    private static IEnumerable<string> ValidateResponseBody(StubModel stub)
    {
        var result = new List<string>();
        var response = stub?.Response;
        if (response == null)
        {
            return result;
        }

        var count = StringHelper.CountNumberOfNonWhitespaceStrings(response.Text, response.Json, response.Xml,
            response.Html, response.Base64, response.File);
        switch (count)
        {
            case 1 when response.StatusCode == (int)HttpStatusCode.NoContent:
                result.Add("When HTTP status code is 204, no response body can be set.");
                break;
            case > 1:
                result.Add(
                    "Only one of the response body fields (text, json, xml, html, base64, file) can be set");
                break;
        }

        return result;
    }

    private static IEnumerable<string> ValidateResponseHeaders(StubModel stub)
    {
        var result = new List<string>();
        var headers = stub?.Response?.Headers;
        if (headers == null || !headers.Any())
        {
            return result;
        }

        result.AddRange(headers
            .Where(h => _illegalHeaders.Contains(h.Key, StringComparer.OrdinalIgnoreCase))
            .Select(h => $"Header '${h.Key}' can't be used as response header."));
        return result;
    }

    private static void ValidateStringRegexReplace(StubModel stub, List<string> validationErrors)
    {

    }
}
