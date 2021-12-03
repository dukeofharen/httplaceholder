using System;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Common.Utilities
{
    public class JsonUtilities
    {
        public static string ConvertFoundValue(JToken jToken)
        {
            var foundValue = jToken switch
            {
                JValue jValue => jValue.ToString(CultureInfo.InvariantCulture),
                JArray jArray when jArray.Any() => jArray.First().ToString(),
                JArray jArray when !jArray.Any() => string.Empty,
                _ => throw new InvalidOperationException(
                    $"JSON type '{jToken.GetType()}' not supported.")
            };
            return foundValue;
        }
    }
}
