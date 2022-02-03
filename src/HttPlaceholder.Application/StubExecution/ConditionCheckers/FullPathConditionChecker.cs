using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
/// Condition checker that is used to validate the full path (so the relative path + query string).
/// </summary>
public class FullPathConditionChecker : IConditionChecker
{
    private readonly IHttpContextService _httpContextService;

    /// <summary>
    /// Constructs a <see cref="BasicAuthenticationConditionChecker"/> instance.
    /// </summary>
    public FullPathConditionChecker(IHttpContextService httpContextService)
    {
        _httpContextService = httpContextService;
    }

    /// <inheritdoc />
    public ConditionCheckResultModel Validate(StubModel stub)
    {
        var result = new ConditionCheckResultModel();
        var fullPathCondition = stub.Conditions?.Url?.FullPath;
        if (string.IsNullOrEmpty(fullPathCondition))
        {
            return result;
        }

        var path = _httpContextService.FullPath;
        if (StringHelper.IsRegexMatchOrSubstring(path, fullPathCondition))
        {
            // The path matches the provided regex. Add the stub ID to the resulting list.
            result.ConditionValidation = ConditionValidationType.Valid;
        }
        else
        {
            result.Log = $"Condition '{fullPathCondition}' did not pass for request.";
            result.ConditionValidation = ConditionValidationType.Invalid;
        }

        return result;
    }

    /// <inheritdoc />
    public int Priority => 9;
}
