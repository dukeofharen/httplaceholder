using System.Collections.Generic;
using HttPlaceholder.Application.Configuration;

namespace HttPlaceholder.Application.Interfaces.Configuration;

/// <summary>
/// Describes a class that contains several configuration related methods.
/// </summary>
public interface IConfigurationHelper
{
    /// <summary>
    /// Returns a list of all possible configuration keys and its metadata.
    /// </summary>
    /// <returns>A list of <see cref="ConfigMetadataModel"/>.</returns>
    IList<ConfigMetadataModel> GetConfigKeyMetadata();
}
