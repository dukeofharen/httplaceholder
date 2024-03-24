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
    : IConditionChecker, ISingletonService
{
    /// <inheritdoc />
    public Task<ConditionCheckResultModel> ValidateAsync(StubModel stub, CancellationToken cancellationToken)
    {
        var hostCondition = stub.Conditions?.Host;
        if (hostCondition == null)
        {
            return NotExecutedAsync();
        }

        return !stringChecker.CheckString(clientDataResolver.GetHost(), hostCondition, out _)
            ? InvalidAsync()
            : ValidAsync();
    }

    /// <inheritdoc />
    public int Priority => 10;
}
