using System.Linq;
using System.Text.RegularExpressions;

namespace HttPlaceholder.Common.Utilities;

/// <summary>
/// A utility class for working with strings.
/// </summary>
public static class StringHelper
{
    /// <summary>
    /// Checks whether a given input either contains a given substring or matches a regex.
    /// </summary>
    /// <param name="fullString">The string to check.</param>
    /// <param name="subStringOrRegex">The substring OR regex expression.</param>
    /// <returns>True if it matched, false if it didn't.</returns>
    public static bool IsRegexMatchOrSubstring(string fullString, string subStringOrRegex)
    {
        var regex = new Regex(subStringOrRegex);
        var result = regex.IsMatch(fullString);
        if (!result)
        {
            result = fullString == subStringOrRegex;
        }

        return result;
    }

    /// <summary>
    /// Ensures that a string ends with a given string.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="append">A piece of text the input should end with.</param>
    /// <returns>The converted input.</returns>
    public static string EnsureEndsWith(this string input, string append)
    {
        if (!input.EndsWith(append))
        {
            return input + append;
        }

        return input;
    }

    /// <summary>
    /// Ensures that a string starts with a given string.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="append">A piece of text the input should start with.</param>
    /// <returns>The converted input.</returns>
    public static string EnsureStartsWith(this string input, string append)
    {
        if (!input.StartsWith(append))
        {
            return append + input;
        }

        return input;
    }

    /// <summary>
    /// A method which receives a list of strings and returns the number of strings which are neither null or whitespace.
    /// </summary>
    /// <param name="strings">The list of strings.</param>
    /// <returns>The number of instances where the string is neither null or whitespace.</returns>
    public static int CountNumberOfNonWhitespaceStrings(params string[] strings) =>
        strings.AsQueryable().Count(s => !string.IsNullOrWhiteSpace(s));
}
