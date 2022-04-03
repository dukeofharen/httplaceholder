using System.Collections.Generic;

namespace HttPlaceholder.Client.Dto.Requests;

/// <summary>
/// A model for storing the response a stub should return.
/// </summary>
public class ResponseDto
{
    /// <summary>
    /// Gets or sets the status code.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Gets or sets the body.
    /// </summary>
    public byte[] Body { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the returned body is binary.
    /// </summary>
    /// <value>
    ///   <c>true</c> if [body is binary]; otherwise, <c>false</c>.
    /// </value>
    public bool BodyIsBinary { get; set; }

    /// <summary>
    /// Gets or sets the headers.
    /// </summary>
    public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>();
}
