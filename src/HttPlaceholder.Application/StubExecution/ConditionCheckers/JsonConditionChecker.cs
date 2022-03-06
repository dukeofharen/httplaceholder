using System;
using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
/// Condition checker that validates if a JSON request body corresponds to a given set of properties.
/// </summary>
public class JsonConditionChecker : IConditionChecker
{
    private readonly IHttpContextService _httpContextService;

    /// <summary>
    /// Constructs a <see cref="JsonConditionChecker"/> instance.
    /// </summary>
    public JsonConditionChecker(IHttpContextService httpContextService)
    {
        _httpContextService = httpContextService;
    }

    /// <inheritdoc />
    public ConditionCheckResultModel Validate(StubModel stub)
    {
        var result = new ConditionCheckResultModel();
        if (stub.Conditions?.Json == null)
        {
            return result;
        }

        var convertedJsonConditions = ConvertJsonConditions(stub.Conditions.Json);

        var body = _httpContextService.GetBody();
        try
        {
            var jToken = JToken.Parse(body);
            var logResults = new List<string>();
            result.ConditionValidation = CheckSubmittedJson(convertedJsonConditions, jToken, logResults)
                ? ConditionValidationType.Valid
                : ConditionValidationType.Invalid;
            result.Log = string.Join(Environment.NewLine, logResults);
        }
        catch (JsonReaderException ex)
        {
            result.ConditionValidation = ConditionValidationType.Invalid;
            result.Log = ex.Message;
        }

        return result;
    }

    /// <inheritdoc />
    public int Priority => 1;

    internal bool CheckSubmittedJson(object input, JToken jToken, List<string> logResults)
    {
        var jtType = jToken.Type;
        if (
            jtType is JTokenType.Null or JTokenType.None or JTokenType.Undefined &&
            input == null)
        {
            return true;
        }

        switch (input)
        {
            case List<object> list:
                return HandleList(list, jToken, logResults);
            case Dictionary<object, object> obj:
                return HandleObject(obj, jToken, logResults);
            case string text:
                return HandleString(text, jToken, logResults);
            default:
                logResults.Add($"Type for input '{input}' ({input.GetType()}) is not supported.");
                return false;
        }
    }

    private bool HandleObject(IDictionary<object, object> obj, JToken jToken, List<string> logging)
    {
        if (jToken.Type != JTokenType.Object)
        {
            logging.Add($"Passed item is of type '{jToken.Type}', but Object was expected.");
            return false;
        }

        var objDict = jToken.ToObject<Dictionary<string, JToken>>();
        if (objDict.Count < obj.Count)
        {
            logging.Add(
                $"Number of elements in posted object ({objDict.Count}) is smaller than the number of configured properties in the stub ({obj.Count}).");
            return false;
        }

        foreach (var pair in obj)
        {
            var foundValue = objDict.FirstOrDefault(o => o.Key.Equals(pair.Key));
            if (!string.IsNullOrWhiteSpace(foundValue.Key))
            {
                if (!CheckSubmittedJson(pair.Value, foundValue.Value, logging))
                {
                    return false;
                }
            }
            else
            {
                logging.Add($"No JSON property found for value '{pair.Key}'.");
                return false;
            }
        }

        return true;
    }

    private bool HandleList(List<object> list, JToken jToken, List<string> logging)
    {
        if (jToken.Type != JTokenType.Array)
        {
            logging.Add($"Passed item is of type '{jToken.Type}', but Array was expected.");
            return false;
        }

        var objList = jToken.ToObject<List<JToken>>();
        if (objList.Count < list.Count)
        {
            logging.Add(
                $"Number of elements in posted list ({objList.Count}) is smaller than the number of configured properties in the stub ({list.Count}).");
            return false;
        }

        // Create a temp list for logging the list results. If the request passes, the logging is added to the final list.
        var tempListLogging = new List<string>();
        var passedConditionCount =
            (from configuredObj in list
                from obj in objList
                where CheckSubmittedJson(configuredObj, obj, tempListLogging)
                select configuredObj).Count();
        var passed = passedConditionCount == objList.Count;
        if (!passed)
        {
            logging.Add(
                $"Number of passed condition in posted list ({passedConditionCount}) doesn't match number of configured items in stub ({list.Count}).");
            logging.AddRange(tempListLogging);
        }

        return passed;
    }

    private static bool HandleString(string text, JToken jToken, ICollection<string> logging)
    {
        switch (jToken.Type)
        {
            case JTokenType.String:
                var passed = StringHelper.IsRegexMatchOrSubstring(jToken.ToString(), text);
                if (!passed)
                {
                    logging.Add($"Input '{jToken}' did not correspond with regex/substring '{text}'.");
                }

                return passed;
            case JTokenType.Boolean:
                var boolInStub = (bool)((JValue)jToken).Value;
                if (bool.TryParse(text, out var parsedBool))
                {
                    return parsedBool == boolInStub;
                }
                else
                {
                    // Handle the boolean that is passed in the JSON as string. The condition might be a regex that needs to be checked.
                    var boolAsString = boolInStub.ToString().ToLower();
                    if (StringHelper.IsRegexMatchOrSubstring(boolAsString, text))
                    {
                        return true;
                    }
                }

                logging.Add($"Value '{text}' not recognized as valid boolean.");
                return false;

            case JTokenType.Float:
                if (double.TryParse(text, out var parsedFloat))
                {
                    return parsedFloat == (double)((JValue)jToken).Value;
                }

                logging.Add($"Value '{text}' not recognized as valid float.");
                return false;

            case JTokenType.Integer:
                if (long.TryParse(text, out var parsedInt))
                {
                    return parsedInt == (long)((JValue)jToken).Value;
                }

                logging.Add($"Value '{text}' not recognized as valid int.");
                return false;

            case JTokenType.Date:
                if (DateTime.TryParse(text, out var parsedDate))
                {
                    return parsedDate == (DateTime)((JValue)jToken).Value;
                }

                logging.Add($"Value '{text}' not recognized as valid date/time.");
                return false;

            default:
                logging.Add($"JSON token type '{jToken.Type}' not supported!");
                return false;
        }
    }

    /// <summary>
    /// Sadly, this method is needed, because YamlDotNet and Newtonsoft.Json both deserialize the JSON condition to another data types.
    /// By calling this method, we are sure that the data is always in the correct format when running this condition checker.
    /// </summary>
    /// <returns>The converted JSON conditions.</returns>
    internal static object ConvertJsonConditions(object conditions)
    {
        switch (conditions)
        {
            case null:
                return null;
            case IDictionary<object, object> or IList<object> or string:
                // Input is already OK, return it directly.
                return conditions;
            case JArray jArray:
            {
                var list = jArray.ToObject<List<object>>();
                return list.Select(ConvertJsonConditions).ToList();
            }
            case JObject jObject:
            {
                var dict = jObject.ToObject<Dictionary<object, object>>();
                return dict.ToDictionary<KeyValuePair<object, object>, object, object>(pair => pair.Key.ToString(), pair => ConvertJsonConditions(pair.Value));
            }
            default:
                return conditions.ToString();
        }
    }
}
