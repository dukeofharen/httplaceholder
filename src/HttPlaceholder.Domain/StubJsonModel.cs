using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Domain
{
    /// <summary>
    /// A model for storing the stub JSON condition model.
    /// </summary>
    public class StubJsonModel
    {
        /// <summary>
        /// Gets or sets whether the posted JSON must be exactly in order as specified in the stub.
        /// </summary>
        [YamlMember(Alias = "shouldBeInOrder")]
        public bool ShouldBeInOrder { get; set; }

        /// <summary>
        /// Gets or sets whether it is allowed for the posted JSON to contain more fields than defined in the stub.
        /// </summary>
        [YamlMember(Alias = "cantContainExtraFields")]
        public bool CantContainExtraFields { get; set; }

        /// <summary>
        /// Gets or sets the JSON properties to check for.
        /// </summary>
        [YamlMember(Alias = "input")]
        public IDictionary<string, object> Input { get; set; }
    }
}
