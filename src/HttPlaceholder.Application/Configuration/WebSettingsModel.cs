namespace HttPlaceholder.Application.Configuration;

/// <summary>
/// A model for storing web related settings.
/// </summary>
public class WebSettingsModel
{
    /// <summary>
    /// Gets or sets the HTTPS port.
    /// </summary>
    public string HttpsPort { get; set; }

    /// <summary>
    /// Gets or sets the HTTP port.
    /// </summary>
    public string HttpPort { get; set; }

    /// <summary>
    /// Gets or sets whether to enable HTTPS.
    /// </summary>
    public bool UseHttps { get; set; }

    /// <summary>
    /// Gets or sets the path to the HttPlaceholder private key.
    /// </summary>
    public string PfxPath { get; set; }

    /// <summary>
    /// Gets or sets the password for the private key.
    /// </summary>
    public string PfxPassword { get; set; }
}
