using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;
using static HttPlaceholder.Domain.ConditionCheckResultModel;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
///     Condition checker that validates the request path (relative path without the query string).
/// </summary>
public class PathConditionChecker(IHttpContextService httpContextService, IStringChecker stringChecker)
    : IConditionChecker, ISingletonService
{
    /// <inheritdoc />
    public Task<ConditionCheckResultModel> ValidateAsync(StubModel stub, CancellationToken cancellationToken)
    {
        var pathCondition = stub.Conditions?.Url?.Path;
        if (pathCondition == null)
        {
            return NotExecutedAsync();
        }

        var path = httpContextService.Path;
        return stringChecker.CheckString(path, pathCondition, out var outputForLogging)
            ? ValidAsync()
            : InvalidAsync($"Condition '{outputForLogging}' did not pass for request.");
    }

    /// <inheritdoc />
    public int Priority => 8;
}
