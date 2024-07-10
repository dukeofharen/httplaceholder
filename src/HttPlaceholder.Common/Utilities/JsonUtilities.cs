using System;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Common.Utilities;

/// <summary>
///     A utility class for working with JSON.
/// </summary>
public static class JsonUtilities
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

    /// <summary>
    ///     Accepts a JToken and performs a JSONPath replace on the JToken.
    ///     Source: https://stackoverflow.com/questions/35874327/newtonsoft-update-jobject-from-json-path (S.Mishra)
    /// </summary>
    /// <param name="root">The root object to find and replace objects in.</param>
    /// <param name="path">The JSONPath.</param>
    /// <param name="newValue">The value that should replace the old value.</param>
    /// <returns>The updated JObject.</returns>
    public static JToken ReplacePath<T>(this JToken root, string path, T newValue)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(path);
        foreach (var value in root.SelectTokens(path).ToList())
        {
            if (value == root)
            {
                root = JToken.FromObject(newValue);
            }
            else
            {
                value.Replace(JToken.FromObject(newValue));
            }
        }

        return root;
    }
}
