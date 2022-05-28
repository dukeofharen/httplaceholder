using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Application.StubExecution.Implementations;

/// <inheritdoc />
internal class StringChecker : IStringChecker
{
    /// <inheritdoc />
    public bool CheckString(string input, object condition)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        if (condition == null)
        {
            throw new ArgumentNullException(nameof(condition));
        }

        if (condition is string stringCondition)
        {
            return StringHelper.IsRegexMatchOrSubstring(input, stringCondition);
        }

        var checkingModel = ConvertCondition(condition);
        var result = true;
        if (!string.IsNullOrWhiteSpace(checkingModel.StringEquals))
        {
            result &= string.Equals(input, checkingModel.StringEquals);
        }

        if (!string.IsNullOrWhiteSpace(checkingModel.StringEqualsCi))
        {
            result &= string.Equals(input, checkingModel.StringEqualsCi, StringComparison.OrdinalIgnoreCase);
        }

        if (!string.IsNullOrWhiteSpace(checkingModel.StringNotEquals))
        {
            result &= !string.Equals(input, checkingModel.StringNotEquals);
        }

        if (!string.IsNullOrWhiteSpace(checkingModel.StringNotEqualsCi))
        {
            result &= !string.Equals(input, checkingModel.StringNotEquals, StringComparison.OrdinalIgnoreCase);
        }

        if (!string.IsNullOrWhiteSpace(checkingModel.Contains))
        {
            result &= input.Contains(checkingModel.Contains);
        }

        if (!string.IsNullOrWhiteSpace(checkingModel.ContainsCi))
        {
            result &= input.Contains(checkingModel.ContainsCi, StringComparison.OrdinalIgnoreCase);
        }

        if (!string.IsNullOrWhiteSpace(checkingModel.NotContains))
        {
            result &= !input.Contains(checkingModel.NotContains);
        }

        if (!string.IsNullOrWhiteSpace(checkingModel.NotContainsCi))
        {
            result &= !input.Contains(checkingModel.NotContainsCi, StringComparison.OrdinalIgnoreCase);
        }

        if (!string.IsNullOrWhiteSpace(checkingModel.StartsWith))
        {
            result &= input.StartsWith(checkingModel.StartsWith);
        }

        if (!string.IsNullOrWhiteSpace(checkingModel.StartsWithCi))
        {
            result &= input.StartsWith(checkingModel.StartsWithCi, StringComparison.OrdinalIgnoreCase);
        }

        if (!string.IsNullOrWhiteSpace(checkingModel.DoesNotStartWith))
        {
            result &= !input.StartsWith(checkingModel.DoesNotEndWith);
        }

        if (!string.IsNullOrWhiteSpace(checkingModel.DoesNotStartWithCi))
        {
            result &= !input.StartsWith(checkingModel.DoesNotStartWithCi, StringComparison.OrdinalIgnoreCase);
        }

        if (!string.IsNullOrWhiteSpace(checkingModel.EndsWith))
        {
            result &= input.EndsWith(checkingModel.EndsWith);
        }

        if (!string.IsNullOrWhiteSpace(checkingModel.EndsWithCi))
        {
            result &= input.EndsWith(checkingModel.EndsWithCi, StringComparison.OrdinalIgnoreCase);
        }

        if (!string.IsNullOrWhiteSpace(checkingModel.DoesNotEndWith))
        {
            result &= !input.EndsWith(checkingModel.DoesNotEndWith);
        }

        if (!string.IsNullOrWhiteSpace(checkingModel.DoesNotEndWithCi))
        {
            result &= !input.EndsWith(checkingModel.DoesNotEndWithCi, StringComparison.OrdinalIgnoreCase);
        }

        if (!string.IsNullOrWhiteSpace(checkingModel.Regex))
        {
            var regex = new Regex(checkingModel.Regex);
            result &= regex.IsMatch(input);
        }

        if (!string.IsNullOrWhiteSpace(checkingModel.RegexNoMatches))
        {
            var regex = new Regex(checkingModel.RegexNoMatches);
            result &= !regex.IsMatch(input);
        }

        return result;
    }

    internal static StubConditionStringCheckingModel ConvertCondition(object condition)
    {
        //Condition can be: JObject (if in cache), Dictionary<object, object> (if added just now)
        if (condition is Dictionary<object, object> dictionary)
        {
            var intermediateJson = JsonConvert.SerializeObject(dictionary);
            return JsonConvert.DeserializeObject<StubConditionStringCheckingModel>(intermediateJson);
        }

        if (condition is JObject jObject)
        {
            return jObject.ToObject<StubConditionStringCheckingModel>();
        }

        throw new InvalidOperationException(
            $"Object of type '{condition.GetType()}' not supported for serializing to '{typeof(StubConditionStringCheckingModel)}'.");
    }
}
