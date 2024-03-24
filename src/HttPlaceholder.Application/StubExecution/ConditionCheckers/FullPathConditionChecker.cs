using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;
using static HttPlaceholder.Domain.ConditionCheckResultModel;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
///     Condition checker that is used to validate the full path (so the relative path + query string).
/// </summary>
public class FullPathConditionChecker(IHttpContextService httpContextService, IStringChecker stringChecker)
    : IConditionChecker, ISingletonService
{
    /// <inheritdoc />
    public Task<ConditionCheckResultModel> ValidateAsync(StubModel stub, CancellationToken cancellationToken)
    {
        var fullPathCondition = stub.Conditions?.Url?.FullPath;
        if (fullPathCondition == null)
        {
            return NotExecutedAsync();
        }

        var path = httpContextService.FullPath;
        return stringChecker.CheckString(path, fullPathCondition, out var outputForLogging)
            ? ValidAsync()
            : InvalidAsync($"Condition '{outputForLogging}' did not pass for request.");
    }

    /// <inheritdoc />
    public int Priority => 9;
}
