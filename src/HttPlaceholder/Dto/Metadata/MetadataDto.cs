using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Dto.Metadata
{
    /// <summary>
    /// A model for storing the HttPlaceholder metadata.
    /// </summary>
    public class MetadataDto : IMapFrom<MetadataModel>
    {
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        public string Version { get; set; }
    }
}
