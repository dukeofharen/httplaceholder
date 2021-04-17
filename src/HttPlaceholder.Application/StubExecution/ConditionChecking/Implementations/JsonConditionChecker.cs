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
            var jsonConditions = conditions?.Json;
            if (jsonConditions == null)
            {
                return result;
            }

            var body = _httpContextService.GetBody();
            var jToken = JToken.Parse(body);
            var logResults = new List<string>();
            var input = jsonConditions.Input;
            result.ConditionValidation = CheckSubmittedJson(input, jToken, logResults)
                ? ConditionValidationType.Valid
                : ConditionValidationType.Invalid;
            result.Log = string.Join(Environment.NewLine, logResults);

            return result;
        }

        public int Priority => 1;

        private bool CheckSubmittedJson(object input, JToken jToken, IList<string> logResults)
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
                    // TODO log when not supported
                    return false;
            }
        }

        private bool HandleObject(IDictionary<object, object> obj, JToken jToken, IList<string> logging)
        {
            if (jToken.Type != JTokenType.Object)
            {
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

        private bool HandleList(IList<object> list, JToken jToken, IList<string> logging)
        {
            if (jToken.Type != JTokenType.Array)
            {
                return false;
            }

            var objList = jToken.ToObject<List<JToken>>();
            if (objList.Count < list.Count)
            {
                logging.Add(
                    $"Number of elements in posted list ({objList.Count}) is smaller than the number of configured properties in the stub ({list.Count}).");
                return false;
            }

            var passedConditionCount =
                (from configuredObj in list
                    from obj in objList
                    where CheckSubmittedJson(configuredObj, obj, logging)
                    select configuredObj).Count();
            var passed = passedConditionCount == objList.Count;
            if (!passed)
            {
                logging.Add($"Number of passed condition in posted list ({passedConditionCount}) doesn't match number of configured items in stub ({list.Count}).");
            }

            return passed;
        }

        private bool HandleString(string text, JToken jToken, IList<string> logging)
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
