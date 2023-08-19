namespace HttPlaceholder.Application.Configuration;

/// <summary>
///     A model for storing web related settings.
/// </summary>
public class WebSettingsModel
{
    /// <summary>
    ///     Gets or sets the HTTPS port.
    /// </summary>
    public string HttpsPort { get; set; }

    /// <summary>
    ///     Gets or sets the HTTP port.
    /// </summary>
    public string HttpPort { get; set; }

    /// <summary>
    ///     Gets or sets whether to enable HTTPS.
    /// </summary>
    public bool UseHttps { get; set; }

    /// <summary>
    ///     Gets or sets the public URL.
    /// </summary>
    public string PublicUrl { get; set; }

    /// <summary>
    ///     Gets or sets the path to the HttPlaceholder private key.
    /// </summary>
    public string PfxPath { get; set; }

    /// <summary>
    ///     Gets or sets the password for the private key.
    /// </summary>
    public string PfxPassword { get; set; }

    /// <summary>
    ///     Gets or sets whether the proxy headers should be read and parsed.
    /// </summary>
    public bool ReadProxyHeaders { get; set; }

    /// <summary>
    ///     Gets or sets a comma delimited list of IPs that are considered to be safe for reading the proxy
    /// </summary>
    public string SafeProxyIps { get; set; }
}
