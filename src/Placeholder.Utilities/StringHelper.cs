using System.Text.RegularExpressions;

namespace Placeholder.Utilities
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
            result = fullString.Contains(subString);
         }

         return result;
      }
   }
}
