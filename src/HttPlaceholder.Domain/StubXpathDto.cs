using System.Collections.Generic;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Domain
{
    /// <summary>
    /// A model for storing information about the XPath condition checker.
    /// </summary>
    public class StubXpathModel
    {
        /// <summary>
        /// Gets or sets the query string.
        /// </summary>
        [YamlMember(Alias = "queryString")]
        [JsonProperty("queryString")]
        public string QueryString { get; set; }

        /// <summary>
        /// Gets or sets the namespaces.
        /// </summary>
        [YamlMember(Alias = "namespaces")]
        [JsonProperty("namespaces")]
        public IDictionary<string, string> Namespaces { get; set; }
    }
}
