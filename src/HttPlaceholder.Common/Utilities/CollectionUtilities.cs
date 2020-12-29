using System;
using System.Collections.Generic;
using System.Linq;

namespace HttPlaceholder.Common.Utilities
{
    public static class CollectionUtilities
    {
        public static void AddOrReplaceCaseInsensitive(this IDictionary<string, string> dict, string key, string value)
        {
            var pair = dict.FirstOrDefault(h => string.Equals(key, h.Key, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(pair.Key))
            {
                dict.Remove(pair.Key);
            }

            dict.Add(key, value);
        }
    }
}
