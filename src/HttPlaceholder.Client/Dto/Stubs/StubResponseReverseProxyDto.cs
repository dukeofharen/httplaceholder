namespace HttPlaceholder.Client.Dto.Stubs;

/// <summary>
///     A model for storing reverse proxy settings.
/// </summary>
public class StubResponseReverseProxyDto
{
    /// <summary>
    ///     Gets or sets the URL where the request should be sent to. The request will be sent to exactly this URL.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    ///     Gets or sets whether the query string of the request to HttPlaceholder should be appended to the string that will
    ///     be send to the proxy URL.
    /// </summary>
    public bool? AppendQueryString { get; set; }

    /// <summary>
    ///     Gets or sets whether the path string of the request to HttPlaceholder should be appended to the string that will be
    ///     send to the proxy URL.
    /// </summary>
    public bool? AppendPath { get; set; }

    /// <summary>
    ///     Gets or sets whether the root URL of the response of the target web service should be replaced with the root URL of
    ///     HttPlaceholder.
    /// </summary>
    public bool? ReplaceRootUrl { get; set; }
}
