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
public class IsHttpsConditionChecker(IClientDataResolver clientDataResolver) : IConditionChecker, ISingletonService
{
    /// <inheritdoc />
    public Task<ConditionCheckResultModel> ValidateAsync(StubModel stub, CancellationToken cancellationToken)
    {
        var condition = stub.Conditions?.Url?.IsHttps;
        if (condition == null)
        {
            return NotExecutedAsync();
        }

        return clientDataResolver.IsHttps() == condition.Value
            ? ValidAsync()
            : InvalidAsync();
    }

    /// <inheritdoc />
    public int Priority => 10;
}
