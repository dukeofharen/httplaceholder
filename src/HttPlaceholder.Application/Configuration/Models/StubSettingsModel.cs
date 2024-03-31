namespace HttPlaceholder.Application.Configuration.Models;

/// <summary>
///     A model for storing stub related settings.
/// </summary>
public class StubSettingsModel
{
    /// <summary>
    ///     Gets or sets the maximum allowed "extra duration" milliseconds.
    /// </summary>
    public int MaximumExtraDurationMillis { get; set; }

    /// <summary>
    ///     Gets or sets whether the root URL is purely used for healthchecking.
    /// </summary>
    public bool HealthcheckOnRootUrl { get; set; }

    /// <summary>
    ///     Gets or sets whether the "file response writer" is allowed to look for files all across the OS.
    ///     If it is set to false, only files relative to the stub .yml files can be used.
    /// </summary>
    public bool AllowGlobalFileSearch { get; set; }
}
