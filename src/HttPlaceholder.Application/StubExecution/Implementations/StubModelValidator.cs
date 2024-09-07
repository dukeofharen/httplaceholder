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
        var extraDuration = stub?.Response?.ExtraDuration;
        var allowedMillis = options.CurrentValue.Stub?.MaximumExtraDurationMillis;
        var parsedDuration = ConversionUtilities.ParseInteger(extraDuration);
        if (parsedDuration.HasValue)
        {
            if (parsedDuration.Value > 0 && parsedDuration.Value > allowedMillis)
            {
                result.Add(string.Format(StubResources.StubValidationExtraDurationTemplate, "ExtraDuration",
                    allowedMillis));
            }
        }
        else if (extraDuration != null)
        {
            var extraDurationModel = ConversionUtilities.Convert<StubExtraDurationModel>(extraDuration);
            if (extraDurationModel?.Min > 0 && extraDurationModel.Min > allowedMillis)
            {
                result.Add(string.Format(StubResources.StubValidationExtraDurationTemplate, "ExtraDuration.Min",
                    allowedMillis));
            }

            if (extraDurationModel?.Max > 0 && extraDurationModel.Max > allowedMillis)
            {
                result.Add(string.Format(StubResources.StubValidationExtraDurationTemplate, "ExtraDuration.Max",
                    allowedMillis));
            }

            if (extraDurationModel is { Min: not null, Max: not null } &&
                extraDurationModel.Min > extraDurationModel.Max)
            {
                result.Add(StubResources.StubValidationMinInvalid);
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
            result.Add(StubResources.StubValidationMinMaxHitsCantBeEqual);
        }

        if (maxHits < minHits)
        {
            result.Add(StubResources.StubValidationMaxHitsCantBeLowerThanMinHits);
        }

        if (exactHits.HasValue && (minHits.HasValue || maxHits.HasValue))
        {
            result.Add(StubResources.StubValidationExactCantBeSet);
        }

        var scenarioResponse = stub?.Response?.Scenario ?? new StubResponseScenarioModel();
        var setScenarioState = scenarioResponse.SetScenarioState;
        var clearState = scenarioResponse.ClearState;
        if (!string.IsNullOrWhiteSpace(setScenarioState) && clearState == true)
        {
            result.Add(StubResources.StubValidationStateInvalid);
        }

        var scenario = stub?.Scenario ?? string.Empty;
        if (string.IsNullOrWhiteSpace(scenario) && (minHits.HasValue || maxHits.HasValue || exactHits.HasValue ||
                                                    !string.IsNullOrWhiteSpace(scenarioState) ||
                                                    !string.IsNullOrWhiteSpace(setScenarioState)))
        {
            result.Add(StubResources.StubValidationScenarioNotProvided);
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
                result.Add(StubResources.StubValidation204WithBody);
                break;
            case > 1:
                result.Add(StubResources.StubValidationMultipleResponseBodyFieldsSet);
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
            .Select(h => string.Format(StubResources.StubValidationHeaderCantBeUsedAsResponseHeader, h.Key)));
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
            if (StringHelper.CountNumberOfNonWhitespaceStrings(replace.Text, replace.Regex, replace.JsonPath) > 1)
            {
                result.Add(string.Format(StubResources.StubValidationStringReplaceMultipleSet, i));
            }

            if (StringHelper.AllAreNullOrWhitespace(replace.Text, replace.Regex, replace.JsonPath))
            {
                result.Add(string.Format(StubResources.StubValidationStringReplaceNoneSet, i));
            }

            if (StringHelper.CountNumberOfNonWhitespaceStrings(replace.Regex, replace.JsonPath) > 0 &&
                replace.IgnoreCase.HasValue)
            {
                result.Add(string.Format(StubResources.StubValidationStringReplaceIgnoreCase, i));
            }

            if (replace.ReplaceWith == null)
            {
                result.Add(string.Format(StubResources.StubValidationReplaceWithNotSet, i));
            }

            i++;
        }

        return result;
    }
}
