﻿using System;
using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Application.StubExecution.ConditionChecking.Implementations
{
    public class JsonConditionChecker : IConditionChecker
    {
        private readonly IHttpContextService _httpContextService;

        public JsonConditionChecker(IHttpContextService httpContextService)
        {
            _httpContextService = httpContextService;
        }

        public ConditionCheckResultModel Validate(string stubId, StubConditionsModel conditions)
        {
            var result = new ConditionCheckResultModel();
            if (conditions?.Json == null)
            {
                return result;
            }

            var body = _httpContextService.GetBody();
            var jToken = JToken.Parse(body);
            var logResults = new List<string>();
            result.ConditionValidation = CheckSubmittedJson(conditions.Json, jToken, logResults)
                ? ConditionValidationType.Valid
                : ConditionValidationType.Invalid;
            result.Log = string.Join(Environment.NewLine, logResults);

            return result;
        }

        public int Priority => 1;

        private bool CheckSubmittedJson(object input, JToken jToken, List<string> logResults)
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

            // Create a temp list for logging the list results. If the request passes, the logging is
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

        private bool HandleString(string text, JToken jToken, List<string> logging)
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
                    if (bool.TryParse(text, out var parsedBool))
                    {
                        return parsedBool == (bool)((JValue)jToken).Value;
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
    }
}
