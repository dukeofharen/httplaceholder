using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;
using static HttPlaceholder.Domain.ConditionCheckResultModel;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
///     Condition checker that verifies if a request is done over HTTP or HTTPS.
/// </summary>
public class IsHttpsConditionChecker(IClientDataResolver clientDataResolver) : BaseConditionChecker, ISingletonService
{
    /// <inheritdoc />
    protected override bool ShouldBeExecuted(StubModel stub) => stub.Conditions?.Url?.IsHttps != null;

    /// <inheritdoc />
    protected override Task<ConditionCheckResultModel> PerformValidationAsync(StubModel stub,
        CancellationToken cancellationToken) =>
        clientDataResolver.IsHttps() == stub.Conditions.Url.IsHttps
            ? ValidAsync()
            : InvalidAsync();

    /// <inheritdoc />
    public override int Priority => 10;
}
