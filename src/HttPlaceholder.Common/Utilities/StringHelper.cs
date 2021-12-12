using System.Text.RegularExpressions;

namespace HttPlaceholder.Common.Utilities;

public static class StringHelper
{
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

    public static string EnsureEndsWith(this string input, string append)
    {
        if (!input.EndsWith(append))
        {
            return input + append;
        }

        return input;
    }

    public static string EnsureStartsWith(this string input, string append)
    {
        if (!input.StartsWith(append))
        {
            return append + input;
        }

        return input;
    }
}