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
    : BaseConditionChecker, ISingletonService
{
    /// <inheritdoc />
    public override int Priority => 8;

    /// <inheritdoc />
    protected override bool ShouldBeExecuted(StubModel stub) => stub.Conditions?.Url?.Path != null;

    /// <inheritdoc />
    protected override Task<ConditionCheckResultModel> PerformValidationAsync(StubModel stub,
        CancellationToken cancellationToken) =>
        stringChecker.CheckString(httpContextService.Path, stub.Conditions.Url.Path, out var outputForLogging)
            ? ValidAsync()
            : InvalidAsync(string.Format(StubResources.PathConditionFailed, outputForLogging));
}
