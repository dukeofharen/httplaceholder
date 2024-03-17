using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
///     Condition checker that validates the request path (relative path without the query string).
/// </summary>
public class PathConditionChecker(IHttpContextService httpContextService, IStringChecker stringChecker) : IConditionChecker, ISingletonService
{
    /// <inheritdoc />
    public Task<ConditionCheckResultModel> ValidateAsync(StubModel stub, CancellationToken cancellationToken)
    {
        var result = new ConditionCheckResultModel();
        var pathCondition = stub.Conditions?.Url?.Path;
        if (pathCondition == null)
        {
            return Task.FromResult(result);
        }

        var path = httpContextService.Path;
        if (stringChecker.CheckString(path, pathCondition, out var outputForLogging))
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
