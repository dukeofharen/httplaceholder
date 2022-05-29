using System.Collections.Generic;

namespace HttPlaceholder.Client.Dto.Stubs
{
    /// <summary>
    /// A model for storing information about the URL condition checkers.
    /// </summary>
    public class StubUrlConditionDto
    {
        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        public object Path { get; set; }

        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        public IDictionary<string, string> Query { get; set; }

        /// <summary>
        /// Gets or sets the full path.
        /// </summary>
        public object FullPath { get; set; }

        /// <summary>
        /// Gets or sets the is HTTPS.
        /// </summary>
        public bool? IsHttps { get; set; }
    }
}
