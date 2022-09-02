using System;
using System.Collections.Generic;
using HttPlaceholder.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Application.StubExecution.Utilities;

/// <summary>
/// A static class that is used to convert a deserialized JSON or YAML object to a DTO.
/// </summary>
public static class ConversionUtilities
{
    /// <summary>
    /// Converts the given input to T.
    /// </summary>
    /// <param name="input">The input that should be converted.</param>
    /// <typeparam name="T">The type the input should be converted to.</typeparam>
    /// <returns>The converted input.</returns>
    public static T Convert<T>(object input)
    {
        switch (input)
        {
            // Input can be: JObject (if in cache), Dictionary<object, object> (if added just now)
            case Dictionary<object, object> dictionary:
            {
                var intermediateJson = JsonConvert.SerializeObject(dictionary);
                return JsonConvert.DeserializeObject<T>(intermediateJson);
            }
            case JObject jObject:
                return jObject.ToObject<T>();
            case T model:
                return model;
            default:
                throw new InvalidOperationException(
                    $"Object of type '{input.GetType()}' not supported for serializing to '{typeof(StubConditionStringCheckingModel)}'.");
        }
    }
}
