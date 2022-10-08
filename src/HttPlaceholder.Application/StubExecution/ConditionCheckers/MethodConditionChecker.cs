using System;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
/// Condition checker to validate the HTTP method.
/// </summary>
public class MethodConditionChecker : IConditionChecker
{
    private readonly IHttpContextService _httpContextService;

    /// <summary>
    /// Constructs a <see cref="MethodConditionChecker"/> instance.
    /// </summary>
    public MethodConditionChecker(IHttpContextService httpContextService)
    {
        _httpContextService = httpContextService;
    }

    /// <inheritdoc />
    public Task<ConditionCheckResultModel> ValidateAsync(StubModel stub, CancellationToken cancellationToken)
    {
        var result = new ConditionCheckResultModel();
        var methodCondition = stub.Conditions?.Method;
        if (string.IsNullOrEmpty(methodCondition))
        {
            return Task.FromResult(result);
        }

        var method = _httpContextService.Method;
        if (string.Equals(methodCondition, method, StringComparison.OrdinalIgnoreCase))
        {
            // The path matches the provided regex. Add the stub ID to the resulting list.
            result.ConditionValidation = ConditionValidationType.Valid;
        }
        else
        {
            result.Log = $"Condition '{methodCondition}' did not pass for request.";
            result.ConditionValidation = ConditionValidationType.Invalid;
        }

        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public int Priority => 10;
}
