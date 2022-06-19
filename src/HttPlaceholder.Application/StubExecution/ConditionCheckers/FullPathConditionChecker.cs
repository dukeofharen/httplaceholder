﻿using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
/// Condition checker that is used to validate the full path (so the relative path + query string).
/// </summary>
public class FullPathConditionChecker : IConditionChecker
{
    private readonly IHttpContextService _httpContextService;
    private readonly IStringChecker _stringChecker;

    /// <summary>
    /// Constructs a <see cref="BasicAuthenticationConditionChecker"/> instance.
    /// </summary>
    public FullPathConditionChecker(IHttpContextService httpContextService, IStringChecker stringChecker)
    {
        _httpContextService = httpContextService;
        _stringChecker = stringChecker;
    }

    /// <inheritdoc />
    public ConditionCheckResultModel Validate(StubModel stub)
    {
        var result = new ConditionCheckResultModel();
        var fullPathCondition = stub.Conditions?.Url?.FullPath;
        if (fullPathCondition == null)
        {
            return result;
        }

        var path = _httpContextService.FullPath;
        if (_stringChecker.CheckString(path, fullPathCondition, out var outputForLogging))
        {
            // The path matches the provided regex. Add the stub ID to the resulting list.
            result.ConditionValidation = ConditionValidationType.Valid;
        }
        else
        {
            result.Log = $"Condition '{outputForLogging}' did not pass for request.";
            result.ConditionValidation = ConditionValidationType.Invalid;
        }

        return result;
    }

    /// <inheritdoc />
    public int Priority => 9;
}
