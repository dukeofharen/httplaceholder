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
    }
}
