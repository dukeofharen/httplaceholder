using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Domain;
using YamlDotNet.Serialization;

namespace HttPlaceholder.Dto.Stubs
{
    /// <summary>
    /// A model for storing metadata of a stub.
    /// </summary>
    public class StubMetadataDto : IMapFrom<StubMetadataModel>, IMapTo<StubMetadataModel>
    {
        /// <summary>
        /// Gets or sets a value indicating whether [read only].
        /// </summary>
        [YamlIgnore]
        public bool ReadOnly { get; set; }
    }
}
