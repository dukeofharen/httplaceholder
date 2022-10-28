using MediatR;

namespace HttPlaceholder.Application.Configuration.Commands.UpdateConfigurationValue;

/// <summary>
///     A command for updating a configuration value runtime.
/// </summary>
public class UpdateConfigurationValueCommand : IRequest
{
    /// <summary>
    ///     Constructs a <see cref="UpdateConfigurationValueCommand" /> object.
    /// </summary>
    public UpdateConfigurationValueCommand(string configurationKey, string newValue)
    {
        ConfigurationKey = configurationKey;
        NewValue = newValue;
    }

    /// <summary>
    ///     The configuration key.
    /// </summary>
    public string ConfigurationKey { get; }

    /// <summary>
    ///     The new configuration value.
    /// </summary>
    public string NewValue { get; }
}
