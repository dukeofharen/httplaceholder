namespace HttPlaceholder.Application.Configuration;

/// <summary>
///     The root model for storing all settings.
/// </summary>
public class SettingsModel
{
    /// <summary>
    ///     Gets or sets the authentication settings.
    /// </summary>
    public AuthenticationSettingsModel Authentication { get; set; }

    /// <summary>
    ///     Gets or sets the web settings.
    /// </summary>
    public WebSettingsModel Web { get; set; }

    /// <summary>
    ///     Gets or sets the storage settings.
    /// </summary>
    public StorageSettingsModel Storage { get; set; }

    /// <summary>
    ///     Gets or sets the user interface settings.
    /// </summary>
    public GuiSettingsModel Gui { get; set; }

    /// <summary>
    ///     Gets or sets the stub settings.
    /// </summary>
    public StubSettingsModel Stub { get; set; }
}
