using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HttPlaceholder.Common.Utilities;

/// <summary>
///     A utility class for working with strings.
/// </summary>
public static class StringHelper
{
    /// <summary>
    ///     Checks whether a given input either contains a given substring or matches a regex.
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
    ///     Ensures that a string ends with a given string.
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
    ///     Ensures that a string doesn't end with a given string.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="character">The character the input should not end with.</param>
    /// <returns>The converted input.</returns>
    public static string EnsureDoesntEndWith(this string input, char character) => input.TrimEnd(character);

    /// <summary>
    ///     Ensures that a string starts with a given string.
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
    ///     A method which receives a list of strings and returns the number of strings which are neither null or whitespace.
    /// </summary>
    /// <param name="strings">The list of strings.</param>
    /// <returns>The number of instances where the string is neither null or whitespace.</returns>
    public static int CountNumberOfNonWhitespaceStrings(params string[] strings) =>
        strings.AsQueryable().Count(s => !string.IsNullOrWhiteSpace(s));

    /// <summary>
    ///     A method which receives a list of strings and checks that all strings are null or whitespace. Returns true if this is the case.
    /// </summary>
    /// <param name="strings">The list of strings.</param>
    /// <returns>True if all strings are null or whitespace; false otherwise.</returns>
    public static bool AllAreNullOrWhitespace(params string[] strings) => strings.All(string.IsNullOrWhiteSpace);

    /// <summary>
    ///     A method which receives a list of strings and checks that no strings are null or whitespace. Returns true if this is the case.
    /// </summary>
    /// <param name="strings">The list of strings.</param>
    /// <returns>True if no strings are null or whitespace; false otherwise.</returns>
    public static bool NoneAreNullOrWhitespace(params string[] strings) => !strings.Where(string.IsNullOrWhiteSpace).Any();

    /// <summary>
    ///     A method which receives a list of strings and checks if any string is null or whitespace. Returns true if this is the case.
    /// </summary>
    /// <param name="strings">The list of strings.</param>
    /// <returns>True if any string is null or whitespace; false otherwise.</returns>
    public static bool AnyAreNullOrWhitespace(params string[] strings) => strings.Any(string.IsNullOrWhiteSpace);

    /// <summary>
    ///     Splits a string on newline characters.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <returns>The input, divided by newline characters.</returns>
    public static string[] SplitNewlines(this string input) =>
        input.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);

    /// <summary>
    ///     Accepts an array of strings and returns the first string that is not empty or whitespace.
    /// </summary>
    /// <param name="input">The strings.</param>
    /// <returns>The first string that contains any other characters than whitespaces. Returns null if nothing is found.</returns>
    public static string GetFirstNonWhitespaceString(params string[] input) =>
        input.FirstOrDefault(str => !string.IsNullOrWhiteSpace(str));

    /// <summary>
    ///     Performs base64 encoding on a given string.
    /// </summary>
    /// <param name="input">The string to encode.</param>
    /// <returns>The base64 encoded result.</returns>
    public static string Base64Encode(this string input) =>
        Convert.ToBase64String(
            Encoding.UTF8.GetBytes(input));
}
