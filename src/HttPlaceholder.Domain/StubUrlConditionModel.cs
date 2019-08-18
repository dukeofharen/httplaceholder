using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Domain
{
    /// <summary>
    /// A model for storing information about the URL condition checkers.
    /// </summary>
    public class StubUrlConditionModel
    {
        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        [YamlMember(Alias = "path")]
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        [YamlMember(Alias = "query")]
        public IDictionary<string, string> Query { get; set; }

        /// <summary>
        /// Gets or sets the full path.
        /// </summary>
        [YamlMember(Alias = "fullPath")]
        public string FullPath { get; set; }

        /// <summary>
        /// Gets or sets the is HTTPS.
        /// </summary>
        [YamlMember(Alias = "isHttps")]
        public bool? IsHttps { get; set; }
    }
}