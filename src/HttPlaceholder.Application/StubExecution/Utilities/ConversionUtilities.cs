using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Application.StubExecution.Utilities;

/// <summary>
///     A static class that is used to convert a deserialized JSON or YAML object to a DTO.
/// </summary>
public static class ConversionUtilities
{
    /// <summary>
    ///     Converts the given input to T.
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
                    $"Object of type '{input.GetType()}' not supported for serializing to '{typeof(T)}'.");
        }
    }

    /// <summary>
    ///     Converts the given input to an enumerable of type T.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <typeparam name="T">The type the input should be converted to.</typeparam>
    /// <returns>The converted input.</returns>
    public static IEnumerable<T> ConvertEnumerable<T>(object input)
    {
        switch (input)
        {
            case JArray jArray:
                return jArray.ToObject<T[]>();
            case IList<object> list:
                return list.Select(i => (T)i);
            default:
                throw new InvalidOperationException(
                    $"Object of type '{input.GetType()}' not supported for serializing to '{typeof(T)}'.");
        }
    }

    /// <summary>
    ///     Accepts an object as input and parsed it to a nullable int.
    /// </summary>
    /// <param name="input">The object to be converted.</param>
    /// <returns>The parsed int, or null if it could not be parsed.</returns>
    public static int? ParseInteger(object input) =>
        input switch
        {
            int inputInt => inputInt,
            long inputLong => (int)inputLong,
            string duration when int.TryParse(duration, out var parsedInput) => parsedInput,
            _ => null
        };
}
