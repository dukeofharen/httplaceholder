using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
///     Condition checker that verifies if a request is done over HTTP or HTTPS.
/// </summary>
public class IsHttpsConditionChecker(IClientDataResolver clientDataResolver) : IConditionChecker, ISingletonService
{
    /// <inheritdoc />
    public Task<ConditionCheckResultModel> ValidateAsync(StubModel stub, CancellationToken cancellationToken)
    {
        var result = new ConditionCheckResultModel();
        var condition = stub.Conditions?.Url?.IsHttps;
        if (condition == null)
        {
            return Task.FromResult(result);
        }

        var shouldBeHttps = condition.Value;
        var isHttps = clientDataResolver.IsHttps();
        result.ConditionValidation = isHttps == shouldBeHttps
            ? ConditionValidationType.Valid
            : ConditionValidationType.Invalid;

        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public int Priority => 10;
}
