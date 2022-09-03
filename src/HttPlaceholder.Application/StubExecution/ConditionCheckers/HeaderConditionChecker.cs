using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.Utilities;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
/// Condition checker that is used to validate the request headers.
/// </summary>
public class HeaderConditionChecker : IConditionChecker
{
    private readonly IHttpContextService _httpContextService;
    private readonly IStringChecker _stringChecker;

    /// <summary>
    /// Constructs a <see cref="HeaderConditionChecker"/> instance.
    /// </summary>
    public HeaderConditionChecker(IHttpContextService httpContextService, IStringChecker stringChecker)
    {
        _httpContextService = httpContextService;
        _stringChecker = stringChecker;
    }

    /// <inheritdoc />
    public Task<ConditionCheckResultModel> ValidateAsync(StubModel stub)
    {
        var result = new ConditionCheckResultModel();
        var headerConditions = stub.Conditions?.Headers;
        if (headerConditions == null || headerConditions.Any() != true)
        {
            return Task.FromResult(result);
        }

        var validHeaders = 0;
        var headers = _httpContextService.GetHeaders();
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
            if (!_stringChecker.CheckString(headerValue, condition.Value, out var outputForLogging))
            {
                // If the check failed, it means the header is incorrect and the condition should fail.
                result.Log = $"Header condition '{condition.Key}: {outputForLogging}' failed.";
                break;
            }

            validHeaders++;
        }

        // If the number of succeeded conditions is equal to the actual number of conditions,
        // the header condition is passed and the stub ID is passed to the result.
        result.ConditionValidation = validHeaders == headerConditions.Count
            ? ConditionValidationType.Valid
            : ConditionValidationType.Invalid;
        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public int Priority => 8;
}
