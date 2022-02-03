namespace HttPlaceholder.Application.Configuration;

/// <summary>
/// Settings model that is used to store the API authentication
/// </summary>
public class AuthenticationSettingsModel
{
    /// <summary>
    /// Gets or sets the API username.
    /// </summary>
    public string ApiUsername { get; set; }

    /// <summary>
    /// Gets or sets the API password.
    /// </summary>
    public string ApiPassword { get; set; }
}
