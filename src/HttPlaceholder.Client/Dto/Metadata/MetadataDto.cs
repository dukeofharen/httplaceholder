using System.Collections.Generic;

namespace HttPlaceholder.Client.Dto.Metadata
{
    /// <summary>
    /// A model for storing the HttPlaceholder metadata.
    /// </summary>
    public class MetadataDto
    {
        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the available variable handlers.
        /// </summary>
        public IEnumerable<VariableHandlerDto> VariableHandlers { get; set; }
    }
}