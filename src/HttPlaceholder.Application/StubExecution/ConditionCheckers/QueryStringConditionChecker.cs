using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.Utilities;
using HttPlaceholder.Domain;
using static HttPlaceholder.Domain.ConditionCheckResultModel;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
///     Condition checker for validating the query strings.
/// </summary>
public class QueryStringConditionChecker(IHttpContextService httpContextService, IStringChecker stringChecker)
    : BaseConditionChecker, ISingletonService
{
    /// <inheritdoc />
    public override int Priority => 8;

    /// <inheritdoc />
    protected override bool ShouldBeExecuted(StubModel stub) => stub.Conditions?.Url?.Query?.Any() == true;

    /// <inheritdoc />
    protected override Task<ConditionCheckResultModel> PerformValidationAsync(StubModel stub,
        CancellationToken cancellationToken)
    {
        var queryStringConditions = stub.Conditions.Url.Query;
        var validQueryStrings = 0;
        var queryString = httpContextService.GetQueryStringDictionary();
        foreach (var condition in queryStringConditions)
        {
            // Do a present check, if needed.
            if (condition.Value is not string)
            {
                var checkingModel = ConversionUtilities.Convert<StubConditionStringCheckingModel>(condition.Value);
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
            if (!stringChecker.CheckString(queryValue, condition.Value, out var outputForLogging))
            {
                // If the check failed, it means the query string is incorrect and the condition should fail.
                return InvalidAsync($"Query string condition '{condition.Key}: {outputForLogging}' failed.");
            }

            validQueryStrings++;
        }

        // If the number of succeeded conditions is equal to the actual number of conditions,
        // the query string condition is passed and the stub ID is passed to the result.
        return validQueryStrings == queryStringConditions.Count
            ? ValidAsync()
            : InvalidAsync();
    }
}
