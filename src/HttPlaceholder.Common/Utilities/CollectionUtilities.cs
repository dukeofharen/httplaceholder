using System;
using System.Collections.Generic;
using System.Linq;

namespace HttPlaceholder.Common.Utilities;

/// <summary>
/// A utility class that contains several collection related methods.
/// </summary>
public static class CollectionUtilities
{
    /// <summary>
    /// Looks for a given value in a dictionary in a case insensitive way and adds the value if it has not been found or updates the value if it has been found.
    /// </summary>
    /// <param name="dict">The dictionary to mutate.</param>
    /// <param name="key">The key to look for.</param>
    /// <param name="value">The value to add.</param>
    /// <param name="replaceIfExists">If set to true, updates the key if it has been found. If set to false, does nothing if the key has been found.</param>
    public static void AddOrReplaceCaseInsensitive(
        this IDictionary<string, string> dict,
        string key,
        string value,
        bool replaceIfExists = true)
    {
        var pair = dict.FirstOrDefault(h => string.Equals(key, h.Key, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(pair.Key))
        {
            if (!replaceIfExists)
            {
                return;
            }

            dict.Remove(pair.Key);
        }

        dict.Add(key, value);
    }

    /// <summary>
    /// Looks for a key-value pair in a dictionary in a case insensitive way.
    /// </summary>
    /// <param name="dict">The dictionary to look in.</param>
    /// <param name="key">The key to look for.</param>
    /// <returns>The found key-value pair.</returns>
    public static KeyValuePair<string, string> CaseInsensitiveSearchPair(this IDictionary<string, string> dict,
        string key) =>
        dict.FirstOrDefault(h => string.Equals(key, h.Key, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Looks for a string value in a dictionary in a case insensitive way.
    /// </summary>
    /// <param name="dict">The dictionary to look in.</param>
    /// <param name="key">The key to look for.</param>
    /// <returns>The found string value.</returns>
    public static string CaseInsensitiveSearch(this IDictionary<string, string> dict, string key)
    {
        var pair = dict.CaseInsensitiveSearchPair(key);
        return string.IsNullOrWhiteSpace(pair.Key) ? null : pair.Value;
    }

    /// <summary>
    /// Checks if a key exists in a dictionary in a case insensitive way.
    /// </summary>
    /// <param name="dict">The dictionary to look in.</param>
    /// <param name="key">The key to look for.</param>
    /// <returns>True if the key was found. False otherwise.</returns>
    public static bool ContainsKeyCaseInsensitive(this IDictionary<string, string> dict, string key) =>
        dict.Keys.Any(k => string.Equals(k, key, StringComparison.OrdinalIgnoreCase));
}
