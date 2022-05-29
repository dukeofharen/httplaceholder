﻿using System;
using System.Collections.Generic;
using HttPlaceholder.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Application.StubExecution.Utilities;

/// <summary>
/// A utility class that is used for working with string conditions.
/// </summary>
public class StringConditionUtilities
{
    /// <summary>
    /// Converts the given condition to <see cref="StubConditionStringCheckingModel"/>.
    /// </summary>
    /// <param name="condition">The condition to convert.</param>
    /// <returns>The converted <see cref="StubConditionStringCheckingModel"/>.</returns>
    public static StubConditionStringCheckingModel ConvertCondition(object condition)
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
