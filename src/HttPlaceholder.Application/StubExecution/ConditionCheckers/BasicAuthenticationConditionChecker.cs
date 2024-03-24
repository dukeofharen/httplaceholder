using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using static HttPlaceholder.Domain.ConditionCheckResultModel;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
///     Condition checker that is used to verify incoming basic authentication credentials.
/// </summary>
public class BasicAuthenticationConditionChecker(IHttpContextService httpContextService)
    : IConditionChecker, ISingletonService
{
    /// <inheritdoc />
    public Task<ConditionCheckResultModel> ValidateAsync(StubModel stub, CancellationToken cancellationToken)
    {
        var condition = stub.Conditions?.BasicAuthentication;
        if (condition == null ||
            StringHelper.AllAreNullOrWhitespace(condition.Username, condition.Password))
        {
            return NotExecutedAsync();
        }

        var headers = httpContextService.GetHeaders();

        // Try to retrieve the Authorization header.
        if (!headers.TryGetCaseInsensitive(HeaderKeys.Authorization, out var authorization))
        {
            return InvalidAsync("No Authorization header found in request.");
        }

        var expectedBase64UsernamePassword = $"{condition.Username}:{condition.Password}".Base64Encode();
        var expectedAuthorizationHeader = $"Basic {expectedBase64UsernamePassword}";
        if (expectedAuthorizationHeader == authorization)
        {
            return ValidAsync($"Basic authentication condition passed for stub '{stub.Id}'.");
        }

        return InvalidAsync(
            $"Basic authentication condition failed for stub '{stub.Id}'. Expected '{expectedAuthorizationHeader}' but found '{authorization}'.");
    }

    /// <inheritdoc />
    public int Priority => 9;
}
