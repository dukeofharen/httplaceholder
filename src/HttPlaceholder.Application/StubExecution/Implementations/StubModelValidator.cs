using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using HttPlaceholder.Application.Configuration.Models;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.Utilities;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Common.Validation;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Application.StubExecution.Implementations;

internal class StubModelValidator(
    IModelValidator modelValidator,
    IOptionsMonitor<SettingsModel> options)
    : IStubModelValidator, ISingletonService
{
    private static readonly string[] _illegalHeaders =
    [
        "X-HttPlaceholder-Correlation", "X-HttPlaceholder-ExecutedStub"
    ];

    /// <inheritdoc />
    public IEnumerable<string> ValidateStubModel(StubModel stub) =>
        HandleValidationResult(modelValidator.ValidateModel(stub))
            .Concat(ValidateExtraDuration(stub))
            .Concat(ValidateScenarioVariables(stub))
            .Concat(ValidateResponseBody(stub))
            .Concat(ValidateResponseHeaders(stub))
            .Concat(ValidateStringRegexReplace(stub));

    private List<string> ValidateExtraDuration(StubModel stub)
    {
        var result = new List<string>();
        const string errorTemplate = "Value for '{0}' cannot be higher than '{1}'.";
        var extraDuration = stub?.Response?.ExtraDuration;
        var allowedMillis = options.CurrentValue.Stub?.MaximumExtraDurationMillis;
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

            if (extraDurationModel is { Min: not null, Max: not null } &&
                extraDurationModel.Min > extraDurationModel.Max)
            {
                result.Add("ExtraDuration.Min should be lower than or equal to ExtraDuration.Max.");
            }
        }

        return result;
    }

    private static List<string> HandleValidationResult(IEnumerable<ValidationResult> validationResults)
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

    private static List<string> ValidateScenarioVariables(StubModel stub)
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

    private static List<string> ValidateResponseBody(StubModel stub)
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

    private static List<string> ValidateResponseHeaders(StubModel stub)
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

    private static List<string> ValidateStringRegexReplace(StubModel stub)
    {
        var result = new List<string>();
        if (stub.Response?.Replace == null)
        {
            return result;
        }

        var i = 0;
        foreach (var replace in stub.Response.Replace)
        {
            if (!string.IsNullOrEmpty(replace.Text) && !string.IsNullOrWhiteSpace(replace.Regex))
            {
                result.Add($"Replace [{i}]: 'text' and 'regex' can't both be set.");
            }

            if (string.IsNullOrEmpty(replace.Text) && string.IsNullOrWhiteSpace(replace.Regex))
            {
                result.Add($"Replace [{i}]: either 'text' or 'regex' needs to be set.");
            }

            if (!string.IsNullOrWhiteSpace(replace.Regex) && replace.IgnoreCase.HasValue)
            {
                result.Add(
                    $"Replace [{i}]: can't set 'ignoreCase' when using 'regex'. This can only be used with 'text'.");
            }

            if (replace.ReplaceWith == null)
            {
                result.Add($"Replace [{i}]: 'replaceWith' should be set.");
            }

            i++;
        }

        return result;
    }
}
