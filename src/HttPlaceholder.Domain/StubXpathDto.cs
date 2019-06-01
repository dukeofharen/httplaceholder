using System.Collections.Generic;
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
        public string QueryString { get; set; }

        /// <summary>
        /// Gets or sets the namespaces.
        /// </summary>
        [YamlMember(Alias = "namespaces")]
        public IDictionary<string, string> Namespaces { get; set; }
    }
}
