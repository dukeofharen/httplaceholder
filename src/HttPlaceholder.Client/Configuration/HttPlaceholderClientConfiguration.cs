using System;
using System.Collections.Generic;

namespace HttPlaceholder.Client.Configuration;

/// <summary>
///     Class that is used to store the HttPlaceholder client configuration.
/// </summary>
public class HttPlaceholderClientConfiguration
{
    private string _rootUrl;

    /// <summary>
    ///     The root URL of the HttPlaceholder instance. The URL will be appended with "/" if it does not end with a "/" yet.
    /// </summary>
    public string RootUrl
    {
        get => _rootUrl;
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _rootUrl = value.EndsWith("/") ? value : $"{value}/";
            }
        }
    }

    /// <summary>
    ///     The username of the HttPlaceholder instance.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    ///     The password of the HttPlaceholder instance.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    ///     Sets a set of default request headers that should be sent with every request through the client.
    /// </summary>
    public IDictionary<string, string> DefaultHttpHeaders { get; set; }

    /// <summary>
    ///     Validates the HttPlaceholder configuration.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(RootUrl))
        {
            throw new ArgumentException(
                $"No value set for {nameof(RootUrl)} in HttPlaceholder configuration.");
        }
    }
}
