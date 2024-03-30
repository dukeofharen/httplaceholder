using System.Collections.Generic;
using System.Linq;

namespace HttPlaceholder.Common.Utilities;

/// <summary>
///     A utility class that is used to parse command line arguments.
/// </summary>
public static class ArgsHelper
{
    /// <summary>
    ///     A method used to parse incoming command line arguments and parse it to a dictionary.
    /// </summary>
    /// <param name="args">The array of command line parameters.</param>
    /// <returns>The parsed dictionary.</returns>
    public static Dictionary<string, string> Parse(this IEnumerable<string> args)
    {
        var subResult = new Dictionary<string, List<string>>();

        var varPointer = string.Empty;
        foreach (var arg in args)
        {
            if (arg.StartsWith("--"))
            {
                varPointer = arg.Replace("--", string.Empty);
                subResult.Add(varPointer, []);
            }
            else
            {
                if (subResult.TryGetValue(varPointer, out var value))
                {
                    value.Add(arg);
                }
            }
        }

        return subResult
            .ToDictionary(d => d.Key, d => string.Join(" ", d.Value));
    }

    /// <summary>
    ///     Adds an entry to the dictionary if it does not exist yet.
    /// </summary>
    /// <param name="args">The dictionary with command line arguments.</param>
    /// <param name="key">The key to check.</param>
    /// <param name="value">The value to add.</param>
    public static void EnsureEntryExists(this IDictionary<string, string> args, string key, object value)
    {
        key = key.ToLower();
        if (!args.ContainsKey(key))
        {
            args.Add(key, value.ToString());
        }
    }
}
