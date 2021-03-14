using System.Collections.Generic;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Domain
{
    /// <summary>
    /// A model for storing all conditions for a stub.
    /// </summary>
    public class StubConditionsModel
    {
        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        [YamlMember(Alias = "method")]
        [JsonProperty("method")]
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        [YamlMember(Alias = "url")]
        [JsonProperty("url")]
        public StubUrlConditionModel Url { get; set; } = new StubUrlConditionModel();

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        [YamlMember(Alias = "body")]
        [JsonProperty("body")]
        public IEnumerable<string> Body { get; set; }

        /// <summary>
        /// Gets or sets the form.
        /// </summary>
        [YamlMember(Alias = "form")]
        [JsonProperty("form")]
        public IEnumerable<StubFormModel> Form { get; set; }

        /// <summary>
        /// Gets or sets the headers.
        /// </summary>
        [YamlMember(Alias = "headers")]
        [JsonProperty("headers")]
        public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the xpath.
        /// </summary>
        [YamlMember(Alias = "xpath")]
        [JsonProperty("xpath")]
        public IEnumerable<StubXpathModel> Xpath { get; set; }

        /// <summary>
        /// Gets or sets the json path.
        /// </summary>
        [YamlMember(Alias = "jsonPath")]
        [JsonProperty("jsonPath")]
        public IEnumerable<string> JsonPath { get; set; }

        /// <summary>
        /// Gets or sets the basic authentication.
        /// </summary>
        [YamlMember(Alias = "basicAuthentication")]
        [JsonProperty("basicAuthentication")]
        public StubBasicAuthenticationModel BasicAuthentication { get; set; }

        /// <summary>
        /// Gets or sets the client ip.
        /// </summary>
        [YamlMember(Alias = "clientIp")]
        [JsonProperty("clientIp")]
        public string ClientIp { get; set; }

        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        [YamlMember(Alias = "host")]
        [JsonProperty("host")]
        public string Host { get; set; }
    }
}
