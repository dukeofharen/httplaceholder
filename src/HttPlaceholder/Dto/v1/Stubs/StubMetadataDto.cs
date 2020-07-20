using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Domain;
using YamlDotNet.Serialization;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global

namespace HttPlaceholder.Dto.v1.Stubs
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
