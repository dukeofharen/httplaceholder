using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using Newtonsoft.Json.Linq;
using static HttPlaceholder.Domain.ConditionCheckResultModel;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
///     Condition checker that validates the incoming JSON request body against a list of JSONPath expressions.
/// </summary>
public class JsonPathConditionChecker(IHttpContextService httpContextService) : BaseConditionChecker, ISingletonService
{
    /// <inheritdoc />
    public override int Priority => 0;

    /// <inheritdoc />
    protected override bool ShouldBeExecuted(StubModel stub) => stub.Conditions?.JsonPath?.Count() > 0;

    /// <inheritdoc />
    protected override async Task<ConditionCheckResultModel> PerformValidationAsync(StubModel stub,
        CancellationToken cancellationToken)
    {
        var jsonPathConditions = stub.Conditions.JsonPath.ToArray();
        var validJsonPaths = 0;
        var body = await httpContextService.GetBodyAsync(cancellationToken);
        var jsonObject = JObject.Parse(body);
        foreach (var condition in jsonPathConditions)
        {
            if (condition is string conditionString)
            {
                // Condition is a string, so perform plain text JSONPath condition check.
                var elements = jsonObject.SelectToken(conditionString);
                if (elements == null)
                {
                    // No suitable JSON results found.
                    return await InvalidAsync(
                        $"No suitable JSON results found with JSONPath query '{conditionString}'.");
                }

                validJsonPaths++;
            }
            else
            {
                // Condition is an object, so first convert the condition to a StubJsonPathModel before executing the condition checker.
                var jsonPathCondition = ConvertJsonPathCondition(stub.Id, condition);
                if (jsonPathCondition == null)
                {
                    continue;
                }

                var elements = jsonObject.SelectToken(jsonPathCondition.Query);
                if (elements == null)
                {
                    continue;
                }

                // Retrieve the value from the JSONPath result.
                var foundValue = JsonUtilities.ConvertFoundValue(elements);

                // If a value is set for the condition, check if the found JSONPath value matches the value in the condition.
                if (!string.IsNullOrWhiteSpace(jsonPathCondition.ExpectedValue) &&
                    !StringHelper.IsRegexMatchOrSubstring(
                        foundValue,
                        jsonPathCondition.ExpectedValue))
                {
                    return await InvalidAsync("No suitable JSON results found.");
                }

                validJsonPaths++;
            }
        }

        // If the number of succeeded conditions is equal to the actual number of conditions,
        // the header condition is passed and the stub ID is passed to the result.
        return validJsonPaths == jsonPathConditions.Length ? await ValidAsync() : await InvalidAsync();
    }

    internal static StubJsonPathModel ConvertJsonPathCondition(string stubId, object condition)
    {
        var jsonPathCondition = condition switch
        {
            JObject conditionObject => conditionObject.ToObject<StubJsonPathModel>(),
            Dictionary<object, object> conditionDict => ParseDict(conditionDict),
            Dictionary<string, string> conditionDict => ParseDict(
                conditionDict.ToDictionary(d => (object)d.Key, d => (object)d.Value)),
            _ => throw new InvalidOperationException(
                $"Can't determine the type of the JSONPath condition for stub with ID '{stubId}'.")
        };
        if (string.IsNullOrWhiteSpace(jsonPathCondition.Query))
        {
            throw new InvalidOperationException(
                $"Value 'query' not set for JSONPath condition for stub with ID '{stubId}'.");
        }

        return jsonPathCondition;

        static StubJsonPathModel ParseDict(IReadOnlyDictionary<object, object> conditionDict)
        {
            return new StubJsonPathModel
            {
                Query = conditionDict.ContainsKey("query")
                    ? conditionDict["query"].ToString()
                    : string.Empty,
                ExpectedValue = conditionDict.ContainsKey("expectedValue")
                    ? conditionDict["expectedValue"].ToString()
                    : string.Empty
            };
        }
    }
}
