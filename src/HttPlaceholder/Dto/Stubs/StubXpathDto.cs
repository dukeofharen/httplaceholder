using System.Collections.Generic;
using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Domain;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Dto.Stubs
{
    /// <summary>
    /// A model for storing information about the XPath condition checker.
    /// </summary>
    public class StubXpathDto : IMapFrom<StubXpathModel>, IMapTo<StubXpathModel>
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
