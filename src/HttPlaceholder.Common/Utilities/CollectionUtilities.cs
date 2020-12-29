using System;
using System.Collections.Generic;
using System.Linq;

namespace HttPlaceholder.Common.Utilities
{
    public static class CollectionUtilities
    {
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

        public static string CaseInsensitiveSearch(this IDictionary<string, string> dict, string key)
        {
            return string.Empty;
        }
    }
}
