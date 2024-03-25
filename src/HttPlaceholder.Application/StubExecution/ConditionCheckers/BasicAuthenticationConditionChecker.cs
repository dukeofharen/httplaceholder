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
    : BaseConditionChecker, ISingletonService
{
    /// <inheritdoc />
    protected override bool ShouldBeExecuted(StubModel stub)
    {
        var condition = stub.Conditions?.BasicAuthentication;
        return condition != null && StringHelper.NoneAreNullOrWhitespace(condition.Username, condition.Password);
    }

    /// <inheritdoc />
    protected override Task<ConditionCheckResultModel> PerformValidationAsync(StubModel stub,
        CancellationToken cancellationToken)
    {
        var condition = stub.Conditions.BasicAuthentication;

        // Try to retrieve the Authorization header.
        var headers = httpContextService.GetHeaders();
        if (!headers.TryGetCaseInsensitive(HeaderKeys.Authorization, out var authorization))
        {
            return InvalidAsync("No Authorization header found in request.");
        }

        var expectedBase64UsernamePassword = $"{condition.Username}:{condition.Password}".Base64Encode();
        var expectedAuthorizationHeader = $"Basic {expectedBase64UsernamePassword}";
        return expectedAuthorizationHeader == authorization
            ? ValidAsync($"Basic authentication condition passed for stub '{stub.Id}'.")
            : InvalidAsync(
                $"Basic authentication condition failed for stub '{stub.Id}'. Expected '{expectedAuthorizationHeader}' but found '{authorization}'.");
    }

    /// <inheritdoc />
    public override int Priority => 9;
}
