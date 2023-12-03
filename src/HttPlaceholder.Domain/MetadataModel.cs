using System.Collections.Generic;

namespace HttPlaceholder.Domain;

/// <summary>
///     A model for storing the HttPlaceholder metadata.
/// </summary>
public class MetadataModel
{
    /// <summary>
    ///     Gets or sets the version.
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    ///     Gets or sets the available variable handlers.
    /// </summary>
    public IEnumerable<VariableHandlerModel> VariableHandlers { get; set; }
}
