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
    : BaseConditionChecker, ISingletonService
{
    /// <inheritdoc />
    public override int Priority => 9;

    /// <inheritdoc />
    protected override bool ShouldBeExecuted(StubModel stub) => stub.Conditions?.Url?.FullPath != null;

    /// <inheritdoc />
    protected override Task<ConditionCheckResultModel> PerformValidationAsync(StubModel stub,
        CancellationToken cancellationToken)
    {
        var fullPathCondition = stub.Conditions.Url.FullPath;
        var path = httpContextService.FullPath;
        return stringChecker.CheckString(path, fullPathCondition, out var outputForLogging)
            ? ValidAsync()
            : InvalidAsync(string.Format(StubResources.FullPathConditionFailed, outputForLogging));
    }
}
