using System;
using System.Collections.Generic;
using System.Linq;

namespace HttPlaceholder.Common.Utilities
{
    public static class ArgsHelper
    {
        public static IDictionary<string, string> Parse(this string[] args)
        {
            var subResult = new Dictionary<string, List<string>>();

            string varPointer = string.Empty;
            foreach (var arg in args)
            {
                if (arg.StartsWith("--"))
                {
                    varPointer = arg.Replace("--", string.Empty).ToLower();
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

        public static string GetValue(this IDictionary<string, string> args, string key)
        {
            args.TryGetValue(key.ToLower(), out string value);
            return value;
        }

        public static string GetValue(this IDictionary<string, string> args, string key, string defaultValue)
        {
            args.TryGetValue(key.ToLower(), out string value);
            return value ?? defaultValue;
        }

        public static string GetAndSetValue(this IDictionary<string, string> args, string key, string defaultValue)
        {
            if(!args.TryGetValue(key.ToLower(), out string value))
            {
                args.Add(key, defaultValue);
            }

            return value ?? defaultValue;
        }

        public static int GetValue(this IDictionary<string, string> args, string key, int defaultValue)
        {
            if (!args.TryGetValue(key.ToLower(), out string value))
            {
                return defaultValue;
            }

            if (!int.TryParse(value, out int result))
            {
                return defaultValue;
            }

            return result;
        }

        public static int GetAndSetValue(this IDictionary<string, string> args, string key, int defaultValue)
        {
            if (!args.TryGetValue(key.ToLower(), out string value))
            {
                args.Add(key, defaultValue.ToString());
                return defaultValue;
            }

            if (!int.TryParse(value, out int result))
            {
                args.Add(key, defaultValue.ToString());
                return defaultValue;
            }

            return result;
        }

        public static bool GetValue(this IDictionary<string, string> args, string key, bool defaultValue)
        {
            if (!args.TryGetValue(key.ToLower(), out string value))
            {
                return defaultValue;
            }

            if (string.Equals(value, "1") || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (string.Equals(value, "0") || string.Equals(value, "false", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return defaultValue;
        }

        public static bool GetAndSetValue(this IDictionary<string, string> args, string key, bool defaultValue)
        {
            if (!args.TryGetValue(key.ToLower(), out string value))
            {
                args.Add(key, defaultValue.ToString());
                return defaultValue;
            }

            if (string.Equals(value, "1") || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (string.Equals(value, "0") || string.Equals(value, "false", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return defaultValue;
        }
    }
}
