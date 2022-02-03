using System.Collections.Generic;

namespace HttPlaceholder.Client.Dto.Stubs
{
    /// <summary>
    /// A model for storing information about the XPath condition checker.
    /// </summary>
    public class StubXpathDto
    {
        /// <summary>
        /// Gets or sets the query string.
        /// </summary>
        public string QueryString { get; set; }

        /// <summary>
        /// Gets or sets the namespaces.
        /// </summary>
        public IDictionary<string, string> Namespaces { get; set; }
    }
}