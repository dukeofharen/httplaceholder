using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
///     Condition checker that is used to verify the hostname.
/// </summary>
public class HostConditionChecker(IClientDataResolver clientDataResolver, IStringChecker stringChecker) : IConditionChecker, ISingletonService
{
    /// <inheritdoc />
    public Task<ConditionCheckResultModel> ValidateAsync(StubModel stub, CancellationToken cancellationToken)
    {
        var result = new ConditionCheckResultModel();
        var hostCondition = stub.Conditions?.Host;
        if (hostCondition == null)
        {
            return Task.FromResult(result);
        }

        var host = clientDataResolver.GetHost();
        result.ConditionValidation = !stringChecker.CheckString(host, hostCondition, out _)
            ? ConditionValidationType.Invalid
            : ConditionValidationType.Valid;

        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public int Priority => 10;
}
