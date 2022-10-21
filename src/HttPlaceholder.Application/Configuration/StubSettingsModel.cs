namespace HttPlaceholder.Application.Configuration;

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
}
