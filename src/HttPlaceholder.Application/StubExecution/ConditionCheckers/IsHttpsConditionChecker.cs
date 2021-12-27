using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
/// Condition checker that verifies if a request is done over HTTP or HTTPS.
/// </summary>
public class IsHttpsConditionChecker : IConditionChecker
{
    private readonly IClientDataResolver _clientDataResolver;

    public IsHttpsConditionChecker(IClientDataResolver clientDataResolver)
    {
        _clientDataResolver = clientDataResolver;
    }

    public ConditionCheckResultModel Validate(StubModel stub)
    {
        var result = new ConditionCheckResultModel();
        var condition = stub.Conditions?.Url?.IsHttps;
        if (condition == null)
        {
            return result;
        }

        var shouldBeHttps = condition.Value;
        var isHttps = _clientDataResolver.IsHttps();
        result.ConditionValidation = isHttps == shouldBeHttps
            ? ConditionValidationType.Valid
            : ConditionValidationType.Invalid;

        return result;
    }

    public int Priority => 10;
}
