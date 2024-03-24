using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;
using static HttPlaceholder.Domain.ConditionCheckResultModel;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
///     Condition checker that is used to verify the hostname.
/// </summary>
public class HostConditionChecker(IClientDataResolver clientDataResolver, IStringChecker stringChecker)
    : BaseConditionChecker, ISingletonService
{
    /// <inheritdoc />
    protected override bool ShouldBeExecuted(StubModel stub) => stub.Conditions?.Host != null;

    /// <inheritdoc />
    protected override Task<ConditionCheckResultModel> PerformValidationAsync(StubModel stub,
        CancellationToken cancellationToken) =>
        !stringChecker.CheckString(clientDataResolver.GetHost(), stub.Conditions.Host, out _)
            ? InvalidAsync()
            : ValidAsync();

    /// <inheritdoc />
    public override int Priority => 10;
}
