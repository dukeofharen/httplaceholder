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

    /// <summary>
    ///     Gets or sets whether the reverse proxy is enabled or disabled.
    /// </summary>
    public bool EnableReverseProxy { get; set; }

    /// <summary>
    ///     Gets or sets a comma-delimited list of allowed hosts. Can either be a full host name, IP address or a regular expression.
    /// </summary>
    public string AllowedHosts { get; set; }

    /// <summary>
    ///     Gets or sets a comma=delimited list of disallowed hosts. Can either be a full host name, IP address or a regular expression.
    /// </summary>
    public string DisallowedHosts { get; set; }
}
