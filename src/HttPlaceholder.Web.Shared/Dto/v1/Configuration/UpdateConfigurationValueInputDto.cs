namespace HttPlaceholder.Web.Shared.Dto.v1.Configuration;

/// <summary>
///     A class used for performing a runtime update on a configuration value
/// </summary>
public class UpdateConfigurationValueInputDto
{
    /// <summary>
    ///     The configuration key.
    /// </summary>
    public string ConfigurationKey { get; set; }

    /// <summary>
    ///     The new configuration value.
    /// </summary>
    public string NewValue { get; set; }
}
