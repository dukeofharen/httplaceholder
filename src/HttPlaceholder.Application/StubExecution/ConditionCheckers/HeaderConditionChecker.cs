using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.Utilities;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using static HttPlaceholder.Domain.ConditionCheckResultModel;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
///     Condition checker that is used to validate the request headers.
/// </summary>
public class HeaderConditionChecker(IHttpContextService httpContextService, IStringChecker stringChecker)
    : BaseConditionChecker, ISingletonService
{
    /// <inheritdoc />
    protected override bool ShouldBeExecuted(StubModel stub) => stub.Conditions?.Headers?.Any() == true;

    /// <inheritdoc />
    protected override Task<ConditionCheckResultModel> PerformValidationAsync(StubModel stub,
        CancellationToken cancellationToken)
    {
        var headerConditions = stub.Conditions.Headers;
        var validHeaders = 0;
        var headers = httpContextService.GetHeaders();
        foreach (var condition in headerConditions)
        {
            // Do a present check, if needed.
            if (condition.Value is not string)
            {
                var checkingModel = ConversionUtilities.Convert<StubConditionStringCheckingModel>(condition.Value);
                if (checkingModel.Present != null)
                {
                    if ((checkingModel.Present.Value && headers.ContainsKeyCaseInsensitive(condition.Key)) ||
                        (!checkingModel.Present.Value && !headers.ContainsKeyCaseInsensitive(condition.Key)))
                    {
                        validHeaders++;
                    }

                    continue;
                }
            }

            // Check whether the condition header is available in the actual headers.
            var headerValue = headers.CaseInsensitiveSearch(condition.Key);
            if (string.IsNullOrWhiteSpace(headerValue))
            {
                continue;
            }

            // Check whether the condition header value is available in the actual headers.
            if (!stringChecker.CheckString(headerValue, condition.Value, out var outputForLogging))
            {
                // If the check failed, it means the header is incorrect and the condition should fail.
                return InvalidAsync($"Header condition '{condition.Key}: {outputForLogging}' failed.");
            }

            validHeaders++;
        }

        // If the number of succeeded conditions is equal to the actual number of conditions,
        // the header condition is passed and the stub ID is passed to the result.
        return validHeaders == headerConditions.Count
            ? ValidAsync()
            : InvalidAsync();
    }

    /// <inheritdoc />
    public override int Priority => 8;
}
