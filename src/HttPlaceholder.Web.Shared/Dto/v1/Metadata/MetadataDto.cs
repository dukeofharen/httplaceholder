using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Web.Shared.Dto.v1.Metadata;

/// <summary>
///     A model for storing the HttPlaceholder metadata.
/// </summary>
public class MetadataDto : IMapFrom<MetadataModel>, IMapTo<MetadataModel>
{
    /// <summary>
    ///     Gets or sets the version.
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    ///     Gets or sets the runtime version.
    /// </summary>
    public string RuntimeVersion { get; set; }

    /// <summary>
    ///     Gets or sets the available variable handlers.
    /// </summary>
    public IEnumerable<VariableHandlerDto> VariableHandlers { get; set; }
}
