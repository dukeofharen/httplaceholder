using System.Collections.Generic;
using System.Net.Http;
using HttPlaceholder.Client.Dto.Stubs;

namespace HttPlaceholder.Client.StubBuilders
{
    /// <summary>
    /// Class for building a <see cref="StubConditionsDto"/> in a fluent way.
    /// </summary>
    public sealed class StubConditionBuilder
    {
        private readonly StubConditionsDto _conditions = new();

        private StubConditionBuilder()
        {
        }

        /// <summary>
        /// Creates a new <see cref="StubConditionBuilder"/> instance.
        /// </summary>
        /// <returns>A <see cref="StubConditionBuilder"/> instance.</returns>
        public static StubConditionBuilder Begin() => new();

        /// <summary>
        /// Sets the HTTP method for the request definition.
        /// </summary>
        /// <param name="method">The HTTP method as string.</param>
        /// <returns>The current <see cref="StubConditionBuilder"/>.</returns>
        public StubConditionBuilder WithHttpMethod(string method)
        {
            _conditions.Method = method;
            return this;
        }

        /// <summary>
        /// Sets the HTTP method for the request definition.
        /// </summary>
        /// <param name="method">The method as <see cref="HttpMethod"/>.</param>
        /// <returns>The current <see cref="StubConditionBuilder"/>.</returns>
        public StubConditionBuilder WithHttpMethod(HttpMethod method)
        {
            _conditions.Method = method.Method;
            return this;
        }

        /// <summary>
        /// Sets the URL path for the request definition.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>The current <see cref="StubConditionBuilder"/>.</returns>
        public StubConditionBuilder WithPath(string path)
        {
            EnsureUrlConditions();
            _conditions.Url.Path = path;
            return this;
        }

        /// <summary>
        /// Adds a query parameter to the request definition.
        /// This method can be called multiple times to add multiple query parameters.
        /// </summary>
        /// <param name="key">The query parameter key.</param>
        /// <param name="value">The query parameter value.</param>
        /// <returns>The current <see cref="StubConditionBuilder"/>.</returns>
        public StubConditionBuilder WithQueryStringParameter(string key, string value)
        {
            EnsureUrlConditions();
            _conditions.Url.Query ??= new Dictionary<string, string>();
            _conditions.Url.Query.Add(key, value);
            return this;
        }

        /// <summary>
        /// Adds a full path to the request definition.
        /// The full path is the path + query parameters.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <returns>The current <see cref="StubConditionBuilder"/>.</returns>
        public StubConditionBuilder WithFullPath(string fullPath)
        {
            EnsureUrlConditions();
            _conditions.Url.FullPath = fullPath;
            return this;
        }

        /// <summary>
        /// Adds a rule to the request definition that the request should be made over HTTPS.
        /// </summary>
        /// <returns>The current <see cref="StubConditionBuilder"/>.</returns>
        public StubConditionBuilder WithHttpsEnabled()
        {
            EnsureUrlConditions();
            _conditions.Url.IsHttps = true;
            return this;
        }

        /// <summary>
        /// Adds a rule to the request definition that the request should be made over HTTP.
        /// </summary>
        /// <returns>The current <see cref="StubConditionBuilder"/>.</returns>
        public StubConditionBuilder WithHttpsDisabled()
        {
            EnsureUrlConditions();
            _conditions.Url.IsHttps = false;
            return this;
        }

        /// <summary>
        /// Adds a check on request body to the request definition.
        /// This method can be called multiple times to add multiple request body conditions.
        /// </summary>
        /// <param name="bodySubstring">The request body to check for.</param>
        /// <returns>The current <see cref="StubConditionBuilder"/>.</returns>
        public StubConditionBuilder WithPostedBodySubstring(string bodySubstring)
        {
            var bodyConditions =
                _conditions.Body != null ? (List<string>)_conditions.Body : new List<string>();
            bodyConditions.Add(bodySubstring);
            _conditions.Body = bodyConditions;
            return this;
        }

        /// <summary>
        /// Adds a check on posted form value to the request definition.
        /// This method can be called multiple times to add multiple posted form conditions.
        /// </summary>
        /// <param name="key">The posted form key.</param>
        /// <param name="value">The posted form value.</param>
        /// <returns>The current <see cref="StubConditionBuilder"/>.</returns>
        public StubConditionBuilder WithPostedFormValue(string key, string value)
        {
            var formConditions = _conditions.Form != null
                ? (List<StubFormDto>)_conditions.Form
                : new List<StubFormDto>();
            formConditions.Add(new StubFormDto {Key = key, Value = value});
            _conditions.Form = formConditions;
            return this;
        }

        /// <summary>
        /// Adds a check on request header to the request definition.
        /// This method can be called multiple times to add multiple request header conditions.
        /// </summary>
        /// <param name="key">The request header key.</param>
        /// <param name="value">The request header value.</param>
        /// <returns>The current <see cref="StubConditionBuilder"/>.</returns>
        public StubConditionBuilder WithRequestHeader(string key, string value)
        {
            _conditions.Headers ??= new Dictionary<string, string>();
            _conditions.Headers.Add(key, value);
            return this;
        }

        /// <summary>
        /// Adds a check on XPath to the request definition. The XPath condition will check the posted XML body on the XPath queries.
        /// This method can be called multiple times to add multiple XPath conditions.
        /// </summary>
        /// <param name="xpath">The XPath query.</param>
        /// <param name="namespaces">The XML namespaces, if necessary.</param>
        /// <returns>The current <see cref="StubConditionBuilder"/>.</returns>
        public StubConditionBuilder WithXPathCondition(string xpath, IDictionary<string, string> namespaces = null)
        {
            var xpathConditions = _conditions.Xpath != null
                ? (List<StubXpathDto>)_conditions.Xpath
                : new List<StubXpathDto>();
            xpathConditions.Add(new StubXpathDto {QueryString = xpath, Namespaces = namespaces});
            _conditions.Xpath = xpathConditions;
            return this;
        }

        /// <summary>
        /// Adds a check on JSONPath to the request definition. The JSONPath condition will check the posted JSON body on the JSONPath queries.
        /// This method can be called multiple times to add multiple JSONPath conditions.
        /// If only the query is filled in, the JSONPath will be checked as is and will pass if any result was found.
        /// If the expectedValue is also filled in, the JSONPath will be checked and the value from the JSONPath query will be checked against the expectedValue.
        /// </summary>
        /// <param name="query">The JSONPath query.</param>
        /// <param name="expectedValue"></param>
        /// <returns>The current <see cref="StubConditionBuilder"/>.</returns>
        public StubConditionBuilder WithJsonPathCondition(string query, string expectedValue = null)
        {
            var jsonpathConditions = _conditions.JsonPath != null
                ? (List<object>)_conditions.JsonPath
                : new List<object>();
            jsonpathConditions.Add(!string.IsNullOrWhiteSpace(expectedValue)
                ? new StubJsonPathModel {Query = query, ExpectedValue = expectedValue}
                : query);
            _conditions.JsonPath = jsonpathConditions;
            return this;
        }

        /// <summary>
        /// Adds a check on basic authentication to the request definition.
        /// </summary>
        /// <param name="username">The basic authentication username.</param>
        /// <param name="password">The basic authentication password.</param>
        /// <returns>The current <see cref="StubConditionBuilder"/>.</returns>
        public StubConditionBuilder WithBasicAuthentication(string username, string password)
        {
            _conditions.BasicAuthentication = new StubBasicAuthenticationDto {Username = username, Password = password};
            return this;
        }

        /// <summary>
        /// Adds a check on client IP address to the request definition.
        /// </summary>
        /// <param name="clientIp">The client IP address.</param>
        /// <returns>The current <see cref="StubConditionBuilder"/>.</returns>
        public StubConditionBuilder WithClientIp(string clientIp)
        {
            _conditions.ClientIp = clientIp;
            return this;
        }

        /// <summary>
        /// Adds a check on client IP in an IP block to the request definition.
        /// </summary>
        /// <param name="ipStartingRange">The starting range of the IP range.</param>
        /// <param name="cidr">The CIDR subnet mask of the IP range.</param>
        /// <returns>The current <see cref="StubConditionBuilder"/>.</returns>
        public StubConditionBuilder WithIpInBlock(string ipStartingRange, string cidr)
        {
            _conditions.ClientIp = $"{ipStartingRange}/{cidr}";
            return this;
        }

        /// <summary>
        /// Adds a check on hostname to the request definition.
        /// </summary>
        /// <param name="hostname">The hostname.</param>
        /// <returns>The current <see cref="StubConditionBuilder"/>.</returns>
        public StubConditionBuilder WithHost(string hostname)
        {
            _conditions.Host = hostname;
            return this;
        }

        /// <summary>
        /// Adds a check on JSON object to the request definition.
        /// </summary>
        /// <param name="jsonObject">The JSON object. This can be any plain old C# object or a dynamic object.</param>
        /// <returns>The current <see cref="StubConditionBuilder"/>.</returns>
        public StubConditionBuilder WithJsonObject(object jsonObject)
        {
            _conditions.Json = jsonObject;
            return this;
        }

        /// <summary>
        /// Adds a check on JSON array to the request definition.
        /// </summary>
        /// <param name="jsonArray">The JSON array. This can be an array that contains plain old C# objects, dynamic objects, strings, ints, booleans etc.</param>
        /// <returns>The current <see cref="StubConditionBuilder"/>.</returns>
        public StubConditionBuilder WithJsonArray(object[] jsonArray)
        {
            _conditions.Json = jsonArray;
            return this;
        }

        /// <summary>
        /// Adds a scenario check to the request definition to check if the scenario has been hit at least minHits (inclusive) times.
        /// </summary>
        /// <param name="minHits">The inclusive minimal hits the scenario should be have been hit.</param>
        /// <returns>The current <see cref="StubConditionBuilder"/>.</returns>
        public StubConditionBuilder ScenarioHasAtLeastXHits(int minHits)
        {
            EnsureScenarioConditions();
            _conditions.Scenario.MinHits = minHits;
            return this;
        }

        /// <summary>
        /// Adds a scenario check to the request definition to check if the scenario has been hit at most maxHits (exclusive) times.
        /// </summary>
        /// <param name="maxHits">The exclusive maximum hits the scenario should have been hit.</param>
        /// <returns>The current <see cref="StubConditionBuilder"/>.</returns>
        public StubConditionBuilder ScenarioHasAtMostXHits(int maxHits)
        {
            EnsureScenarioConditions();
            _conditions.Scenario.MaxHits = maxHits;
            return this;
        }

        /// <summary>
        /// Adds a scenario check to the request definition to check if the scenario has been hit exactly exactHits times.
        /// </summary>
        /// <param name="exactHits">The number of times the scenario should have been hit.</param>
        /// <returns>The current <see cref="StubConditionBuilder"/>.</returns>
        public StubConditionBuilder ScenarioHasExactlyXHits(int exactHits)
        {
            EnsureScenarioConditions();
            _conditions.Scenario.ExactHits = exactHits;
            return this;
        }

        /// <summary>
        /// Adds a scenario check to the request definition to check if the scenario is in a specific state.
        /// </summary>
        /// <param name="state">The state the scenario should be in.</param>
        /// <returns>The current <see cref="StubConditionBuilder"/>.</returns>
        public StubConditionBuilder ScenarioHasState(string state)
        {
            EnsureScenarioConditions();
            _conditions.Scenario.ScenarioState = state;
            return this;
        }

        /// <summary>
        /// Builds the stub conditions.
        /// </summary>
        /// <returns>The built <see cref="StubConditionsDto"/>.</returns>
        public StubConditionsDto Build() => _conditions;

        private void EnsureUrlConditions() => _conditions.Url ??= new StubUrlConditionDto();

        private void EnsureScenarioConditions() => _conditions.Scenario ??= new StubConditionScenarioDto();
    }
}
