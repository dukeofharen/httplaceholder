﻿using System;
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
///     Condition checker that validates if a JSON request body corresponds to a given set of properties.
/// </summary>
public class JsonConditionChecker(IHttpContextService httpContextService) : BaseConditionChecker, ISingletonService
{
    /// <inheritdoc />
    public override int Priority => 1;

    /// <inheritdoc />
    protected override bool ShouldBeExecuted(StubModel stub) => stub.Conditions?.Json != null;

    /// <inheritdoc />
    protected override async Task<ConditionCheckResultModel> PerformValidationAsync(StubModel stub,
        CancellationToken cancellationToken)
    {
        var convertedJsonConditions = ConvertJsonConditions(stub.Conditions.Json);
        var jToken = JToken.Parse(await httpContextService.GetBodyAsync(cancellationToken));
        var logResults = new List<string>();
        return CheckSubmittedJson(convertedJsonConditions, jToken, logResults)
            ? await ValidAsync(string.Join(Environment.NewLine, logResults))
            : await InvalidAsync(string.Join(Environment.NewLine, logResults));
    }

    internal bool CheckSubmittedJson(object input, JToken jToken, List<string> logging)
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
                return HandleList(list, jToken, logging);
            case Dictionary<object, object> obj:
                return HandleObject(obj, jToken, logging);
            case string text:
                return HandleString(text, jToken, logging);
            default:
                logging.Add(string.Format(StubResources.JsonTypeNotSupported, input, input.GetType()));
                return false;
        }
    }

    private bool HandleObject(IDictionary<object, object> obj, JToken jToken, List<string> logging)
    {
        if (jToken.Type != JTokenType.Object)
        {
            logging.Add(string.Format(StubResources.JsonPassedItemObjectExpected, jToken.Type));
            return false;
        }

        var objDict = jToken.ToObject<Dictionary<string, JToken>>();
        if (objDict.Count < obj.Count)
        {
            logging.Add(string.Format(StubResources.JsonNumberOfItemsTooSmall, objDict.Count, obj.Count));
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
                logging.Add(string.Format(StubResources.JsonPropertyNotFound, pair.Key));
                return false;
            }
        }

        return true;
    }

    private bool HandleList(List<object> list, JToken jToken, List<string> logging)
    {
        if (jToken.Type != JTokenType.Array)
        {
            logging.Add(string.Format(StubResources.JsonPassedItemArrayExpected, jToken.Type));
            return false;
        }

        var objList = jToken.ToObject<List<JToken>>();
        if (objList.Count < list.Count)
        {
            logging.Add(string.Format(StubResources.JsonNumberOfItemsTooSmall, objList.Count, list.Count));
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
            logging.Add(string.Format(StubResources.JsonNumberOfItemsDoesntMatch, passedConditionCount, list.Count));
            logging.AddRange(tempListLogging);
        }

        return passed;
    }

    private static bool HandleString(string text, JToken jToken, List<string> logging)
    {
        switch (jToken.Type)
        {
            case JTokenType.String:
                var passed = StringHelper.IsRegexMatchOrSubstring(jToken.ToString(), text);
                if (!passed)
                {
                    logging.Add(
                        string.Format(StubResources.JsonInputDoesntCorrespondWithRegexOrSubstring, jToken, text));
                }

                return passed;
            case JTokenType.Boolean:
                var boolInStub = (bool)((JValue)jToken).Value;
                if (bool.TryParse(text, out var parsedBool))
                {
                    return parsedBool == boolInStub;
                }

                // Handle the boolean that is passed in the JSON as string. The condition might be a regex that needs to be checked.
                var boolAsString = boolInStub.ToString().ToLower();
                if (StringHelper.IsRegexMatchOrSubstring(boolAsString, text))
                {
                    return true;
                }

                logging.Add(string.Format(StubResources.JsonValueInvalidBoolean, text));
                return false;

            case JTokenType.Float:
                if (double.TryParse(text, out var parsedFloat))
                {
                    return parsedFloat == (double)((JValue)jToken).Value;
                }

                logging.Add(string.Format(StubResources.JsonValueInvalidFloat, text));
                return false;

            case JTokenType.Integer:
                if (long.TryParse(text, out var parsedInt))
                {
                    return parsedInt == (long)((JValue)jToken).Value;
                }

                logging.Add(string.Format(StubResources.JsonValueInvalidInt, text));
                return false;

            case JTokenType.Date:
                if (DateTime.TryParse(text, out var parsedDate))
                {
                    return parsedDate == (DateTime)((JValue)jToken).Value;
                }

                logging.Add(string.Format(StubResources.JsonValueInvalidDateTime, text));
                return false;

            default:
                logging.Add(string.Format(StubResources.JsonTokenTypeNotSupported, jToken.Type));
                return false;
        }
    }

    /// <summary>
    ///     Sadly, this method is needed, because YamlDotNet and Newtonsoft.Json both deserialize the JSON condition to another
    ///     data types.
    ///     By calling this method, we are sure that the data is always in the correct format when running this condition
    ///     checker.
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
                return dict.ToDictionary<KeyValuePair<object, object>, object, object>(pair => pair.Key.ToString(),
                    pair => ConvertJsonConditions(pair.Value));
            }
            default:
                return conditions.ToString();
        }
    }
}
