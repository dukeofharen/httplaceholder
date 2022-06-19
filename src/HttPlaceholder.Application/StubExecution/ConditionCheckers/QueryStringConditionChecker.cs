using System.Linq;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
/// Condition checker for validating the query strings.
/// </summary>
public class QueryStringConditionChecker : IConditionChecker
{
    private readonly IHttpContextService _httpContextService;
    private readonly IStringChecker _stringChecker;

    /// <summary>
    /// Constructs a <see cref="QueryStringConditionChecker"/> instance.
    /// </summary>
    public QueryStringConditionChecker(IHttpContextService httpContextService, IStringChecker stringChecker)
    {
        _httpContextService = httpContextService;
        _stringChecker = stringChecker;
    }

    /// <inheritdoc />
    public ConditionCheckResultModel Validate(StubModel stub)
    {
        var result = new ConditionCheckResultModel();
        var queryStringConditions = stub.Conditions?.Url?.Query;
        if (queryStringConditions == null || queryStringConditions.Any() != true)
        {
            return result;
        }

        var validQueryStrings = 0;
        var queryString = _httpContextService.GetQueryStringDictionary();
        foreach (var condition in queryStringConditions)
        {
            // Do a present check, if needed.
            if (condition.Value is not string)
            {
                var checkingModel = StringConditionUtilities.ConvertCondition(condition.Value);
                if (checkingModel.Present != null)
                {
                    if ((checkingModel.Present.Value && queryString.ContainsKey(condition.Key)) ||
                        (!checkingModel.Present.Value && !queryString.ContainsKey(condition.Key)))
                    {
                        validQueryStrings++;
                    }

                    continue;
                }
            }

            // Check whether the condition query is available in the actual query string.
            if (!queryString.TryGetValue(condition.Key, out var queryValue))
            {
                continue;
            }

            // Check whether the condition query value is available in the actual query string.
            if (!_stringChecker.CheckString(queryValue, condition.Value, out var outputForLogging))
            {
                // If the check failed, it means the query string is incorrect and the condition should fail.
                result.Log = $"Query string condition '{condition.Key}: {outputForLogging}' failed.";
                break;
            }

            validQueryStrings++;
        }

        // If the number of succeeded conditions is equal to the actual number of conditions,
        // the query string condition is passed and the stub ID is passed to the result.
        result.ConditionValidation = validQueryStrings == queryStringConditions.Count
            ? ConditionValidationType.Valid
            : ConditionValidationType.Invalid;

        return result;
    }

    /// <inheritdoc />
    public int Priority => 8;
}
