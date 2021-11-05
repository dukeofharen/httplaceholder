using System.Collections.Generic;
using System.Net.Http;
using HttPlaceholder.Client.Dto.Stubs;

namespace HttPlaceholder.Client.StubBuilders
{
    /// <summary>
    /// Class for building the stub conditions.
    /// </summary>
    public sealed class StubConditionBuilder
    {
        private readonly StubConditionsDto _conditions = new();

        private StubConditionBuilder()
        {
        }

        public static StubConditionBuilder Begin() => new();

        public StubConditionBuilder WithHttpMethod(string method)
        {
            _conditions.Method = method;
            return this;
        }

        public StubConditionBuilder WithHttpMethod(HttpMethod method)
        {
            _conditions.Method = method.Method;
            return this;
        }

        public StubConditionBuilder WithPath(string path)
        {
            EnsureUrlConditions();
            _conditions.Url.Path = path;
            return this;
        }

        public StubConditionBuilder WithQueryStringParameter(string key, string value)
        {
            EnsureUrlConditions();
            _conditions.Url.Query ??= new Dictionary<string, string>();
            _conditions.Url.Query.Add(key, value);
            return this;
        }

        public StubConditionBuilder WithFullPath(string fullPath)
        {
            EnsureUrlConditions();
            _conditions.Url.FullPath = fullPath;
            return this;
        }

        public StubConditionBuilder WithHttpsEnabled()
        {
            EnsureUrlConditions();
            _conditions.Url.IsHttps = true;
            return this;
        }

        public StubConditionBuilder WithHttpsDisabled()
        {
            EnsureUrlConditions();
            _conditions.Url.IsHttps = false;
            return this;
        }

        public StubConditionBuilder WithPostedBodySubstring(string bodySubstring)
        {
            var bodyConditions =
                _conditions.Body != null ? (List<string>)_conditions.Body : new List<string>();
            bodyConditions.Add(bodySubstring);
            _conditions.Body = bodyConditions;
            return this;
        }

        public StubConditionBuilder WithPostedFormValue(string key, string value)
        {
            var formConditions = _conditions.Form != null
                ? (List<StubFormDto>)_conditions.Form
                : new List<StubFormDto>();
            formConditions.Add(new StubFormDto {Key = key, Value = value});
            _conditions.Form = formConditions;
            return this;
        }

        public StubConditionBuilder WithRequestHeader(string key, string value)
        {
            _conditions.Headers ??= new Dictionary<string, string>();
            _conditions.Headers.Add(key, value);
            return this;
        }

        public StubConditionBuilder WithXPathCondition(string xpath, IDictionary<string, string> namespaces = null)
        {
            var xpathConditions = _conditions.Xpath != null
                ? (List<StubXpathDto>)_conditions.Xpath
                : new List<StubXpathDto>();
            xpathConditions.Add(new StubXpathDto {QueryString = xpath, Namespaces = namespaces});
            _conditions.Xpath = xpathConditions;
            return this;
        }

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

        public StubConditionBuilder WithBasicAuthentication(string username, string password)
        {
            _conditions.BasicAuthentication = new StubBasicAuthenticationDto {Username = username, Password = password};
            return this;
        }

        public StubConditionBuilder WithClientIp(string clientIp)
        {
            _conditions.ClientIp = clientIp;
            return this;
        }

        public StubConditionBuilder WithIpInBlock(string ipStartingRange, string cidr)
        {
            _conditions.ClientIp = $"{ipStartingRange}/{cidr}";
            return this;
        }

        public StubConditionBuilder WithHost(string hostname)
        {
            _conditions.Host = hostname;
            return this;
        }

        public StubConditionBuilder WithJsonObject(object jsonObject)
        {
            _conditions.Json = jsonObject;
            return this;
        }

        public StubConditionBuilder WithJsonArray(object[] jsonArray)
        {
            _conditions.Json = jsonArray;
            return this;
        }

        public StubConditionBuilder ScenarioHasAtLeastXHits(int minHits)
        {
            EnsureScenarioConditions();
            _conditions.Scenario.MinHits = minHits;
            return this;
        }

        public StubConditionBuilder ScenarioHasAtMostXHits(int maxHits)
        {
            EnsureScenarioConditions();
            _conditions.Scenario.MaxHits = maxHits;
            return this;
        }

        public StubConditionBuilder ScenarioHasExactlyXHits(int exactHits)
        {
            EnsureScenarioConditions();
            _conditions.Scenario.ExactHits = exactHits;
            return this;
        }

        public StubConditionBuilder ScenarioHasState(string state)
        {
            EnsureScenarioConditions();
            _conditions.Scenario.ScenarioState = state;
            return this;
        }

        public StubConditionsDto Build() => _conditions;

        private void EnsureUrlConditions() => _conditions.Url ??= new StubUrlConditionDto();

        private void EnsureScenarioConditions() => _conditions.Scenario ??= new StubConditionScenarioDto();
    }
}
