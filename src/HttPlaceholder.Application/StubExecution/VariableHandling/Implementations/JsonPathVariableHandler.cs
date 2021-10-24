using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Application.StubExecution.VariableHandling.Implementations
{
    public class JsonPathVariableHandler : IVariableHandler
    {
        private readonly IHttpContextService _httpContextService;
        private readonly ILogger<JsonPathVariableHandler> _logger;

        public JsonPathVariableHandler(
            IHttpContextService httpContextService,
            ILogger<JsonPathVariableHandler> logger)
        {
            _httpContextService = httpContextService;
            _logger = logger;
        }

        public string Name => "jsonpath";

        public string FullName => "JSONPath variable handler";

        public string Example => "((jsonpath:$.values[1].title))";

        public string Parse(string input, IEnumerable<Match> matches)
        {
            var matchArray = matches as Match[] ?? matches.ToArray();
            if (!matchArray.Any())
            {
                return input;
            }

            var body = _httpContextService.GetBody();
            JObject jsonObject = null;
            try
            {
                jsonObject = JObject.Parse(body);
            }
            catch (JsonException je)
            {
                _logger.LogInformation("Exception occurred while trying to parse response body as JSON.", je);
            }

            return matchArray
                .Where(match => match.Groups.Count >= 2)
                .Aggregate(input,
                    (current, match) => current.Replace(match.Value, GetJsonPathValue(match, jsonObject)));
        }

        private string GetJsonPathValue(Match match, JToken token)
        {
            var jsonPathQuery = match.Groups[2].Value;
            var foundValue = token?.SelectToken(jsonPathQuery);
            return foundValue != null ? JsonUtilities.ConvertFoundValue(foundValue) : string.Empty;
        }
    }
}
