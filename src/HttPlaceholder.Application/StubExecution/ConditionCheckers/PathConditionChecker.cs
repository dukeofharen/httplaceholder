﻿using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
///     Condition checker that validates the request path (relative path without the query string).
/// </summary>
public class PathConditionChecker : IConditionChecker, ISingletonService
{
    private readonly IHttpContextService _httpContextService;
    private readonly IStringChecker _stringChecker;

    /// <summary>
    ///     Constructs a <see cref="PathConditionChecker" /> instance.
    /// </summary>
    public PathConditionChecker(IHttpContextService httpContextService, IStringChecker stringChecker)
    {
        _httpContextService = httpContextService;
        _stringChecker = stringChecker;
    }

    /// <inheritdoc />
    public Task<ConditionCheckResultModel> ValidateAsync(StubModel stub, CancellationToken cancellationToken)
    {
        var result = new ConditionCheckResultModel();
        var pathCondition = stub.Conditions?.Url?.Path;
        if (pathCondition == null)
        {
            return Task.FromResult(result);
        }

        var path = _httpContextService.Path;
        if (_stringChecker.CheckString(path, pathCondition, out var outputForLogging))
        {
            // The path matches.
            result.ConditionValidation = ConditionValidationType.Valid;
        }
        else
        {
            result.Log = $"Condition '{outputForLogging}' did not pass for request.";
            result.ConditionValidation = ConditionValidationType.Invalid;
        }

        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public int Priority => 8;
}
