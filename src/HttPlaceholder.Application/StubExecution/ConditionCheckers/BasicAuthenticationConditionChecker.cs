using System;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
/// Condition checker that is used to verify incoming basic authentication credentials.
/// </summary>
public class BasicAuthenticationConditionChecker : IConditionChecker
{
    private readonly IHttpContextService _httpContextService;

    /// <summary>
    /// Constructs a <see cref="BasicAuthenticationConditionChecker"/> instance.
    /// </summary>
    public BasicAuthenticationConditionChecker(IHttpContextService httpContextService)
    {
        _httpContextService = httpContextService;
    }

    /// <inheritdoc />
    public Task<ConditionCheckResultModel> ValidateAsync(StubModel stub)
    {
        var result = new ConditionCheckResultModel();
        var basicAuthenticationCondition = stub.Conditions?.BasicAuthentication;
        if (basicAuthenticationCondition == null ||
            (string.IsNullOrWhiteSpace(basicAuthenticationCondition.Username) &&
             string.IsNullOrWhiteSpace(basicAuthenticationCondition.Password)))
        {
            return Task.FromResult(result);
        }

        var headers = _httpContextService.GetHeaders();

        // Try to retrieve the Authorization header.
        if (!headers.TryGetValue("Authorization", out var authorization))
        {
            result.ConditionValidation = ConditionValidationType.Invalid;
            result.Log = "No Authorization header found in request.";
        }
        else
        {
            var expectedBase64UsernamePassword = Convert.ToBase64String(
                Encoding.UTF8.GetBytes(
                    $"{basicAuthenticationCondition.Username}:{basicAuthenticationCondition.Password}"));
            var expectedAuthorizationHeader = $"Basic {expectedBase64UsernamePassword}";
            if (expectedAuthorizationHeader == authorization)
            {
                result.Log = $"Basic authentication condition passed for stub '{stub.Id}'.";
                result.ConditionValidation = ConditionValidationType.Valid;
            }
            else
            {
                result.Log =
                    $"Basic authentication condition failed for stub '{stub.Id}'. Expected '{expectedAuthorizationHeader}' but found '{authorization}'.";
                result.ConditionValidation = ConditionValidationType.Invalid;
            }
        }

        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public int Priority => 9;
}
