using System;
using System.Collections.Generic;
using System.Text;

namespace HttPlaceholder.Client.Dto.Requests;

/// <summary>
///     A model for storing the request data for a request.
/// </summary>
public class RequestParametersDto
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
    ///     Gets or sets the body.
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    ///     Gets or sets whether the request body is binary.
    /// </summary>
    public bool BodyIsBinary { get; set; }

    /// <summary>
    ///     This method returns the request body as byte array. If the body is not binary, a UTF8 encoded byte array of the string body is returned.
    /// </summary>
    /// <returns>The body as byte array.</returns>
    public byte[] GetBodyAsBytes() => BodyIsBinary ? Convert.FromBase64String(Body) : Encoding.UTF8.GetBytes(Body);

    /// <summary>
    ///     Gets or sets the headers.
    /// </summary>
    public IDictionary<string, string> Headers { get; set; }

    /// <summary>
    ///     Gets or sets the client ip.
    /// </summary>
    public string ClientIp { get; set; }
}
