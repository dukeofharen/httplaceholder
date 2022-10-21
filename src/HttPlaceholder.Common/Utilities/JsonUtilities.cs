using System;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Common.Utilities;

/// <summary>
///     A utility class for working with JSON.
/// </summary>
public class JsonUtilities
{
    /// <summary>
    ///     Converts a given <see cref="JToken" /> to a string.
    /// </summary>
    /// <param name="jToken">The <see cref="JToken" />.</param>
    /// <returns>The converted JSON as string.</returns>
    /// <exception cref="InvalidOperationException">If the type of <see cref="JToken" /> is not supported.</exception>
    public static string ConvertFoundValue(JToken jToken)
    {
        var foundValue = jToken switch
        {
            JValue jValue => jValue.ToString(CultureInfo.InvariantCulture),
            JArray jArray when jArray.Any() => jArray.First().ToString(),
            JArray jArray when !jArray.Any() => string.Empty,
            _ => throw new InvalidOperationException(
                $"JSON type '{jToken.GetType()}' not supported.")
        };
        return foundValue;
    }
}
