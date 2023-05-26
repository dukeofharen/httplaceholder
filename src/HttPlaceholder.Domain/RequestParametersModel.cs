using System.Collections.Generic;

namespace HttPlaceholder.Domain;

/// <summary>
///     A model for storing the request data for a request.
/// </summary>
public class RequestParametersModel
{
    /// <summary>
    ///     Gets or sets the method.
    /// </summary>
    public string Method { get; set; }

    /// <summary>
    ///     Gets or sets the URL.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    ///     Gets or sets the binary body.
    /// </summary>
    public byte[] BinaryBody { get; set; }

    /// <summary>
    ///     Gets or sets the headers.
    /// </summary>
    public IDictionary<string, string> Headers { get; set; }

    /// <summary>
    ///     Gets or sets the client ip.
    /// </summary>
    public string ClientIp { get; set; }
}
