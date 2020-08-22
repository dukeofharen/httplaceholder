using System.Text.RegularExpressions;

namespace HttPlaceholder.Common.Utilities
{
    public static class StringHelper
    {
        public static bool IsRegexMatchOrSubstring(string fullString, string subString)
        {
            bool result;
            var regex = new Regex(subString);
            result = regex.IsMatch(fullString);
            if (!result)
            {
                result = fullString == subString;
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
}
