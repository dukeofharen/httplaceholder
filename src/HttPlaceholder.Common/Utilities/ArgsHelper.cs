using System.Collections.Generic;
using System.Linq;

namespace HttPlaceholder.Common.Utilities
{
    public static class ArgsHelper
    {
        public static IDictionary<string, string> Parse(this IEnumerable<string> args)
        {
            var subResult = new Dictionary<string, List<string>>();

            var varPointer = string.Empty;
            foreach (var arg in args)
            {
                if (arg.StartsWith("--"))
                {
                    varPointer = arg.Replace("--", string.Empty);
                    subResult.Add(varPointer, new List<string>());
                }
                else
                {
                    if (subResult.ContainsKey(varPointer))
                    {
                        subResult[varPointer].Add(arg);
                    }
                }
            }

            return subResult
                .ToDictionary(d => d.Key, d => string.Join(" ", d.Value));
        }

        public static void EnsureEntryExists(this IDictionary<string, string> args, string key, object value)
        {
            key = key.ToLower();
            if (!args.ContainsKey(key))
            {
                args.Add(key, value.ToString());
            }
        }
    }
}
