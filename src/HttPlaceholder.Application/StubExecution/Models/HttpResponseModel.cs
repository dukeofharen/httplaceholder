using System.Collections.Generic;

namespace HttPlaceholder.Application.StubExecution.Models;

/// <summary>
/// A model that contains a representation of an HTTP response.
/// </summary>
public class HttpResponseModel
{
    /// <summary>
    /// Gets or sets the HTTP status code.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Gets or sets the headers.
    /// </summary>
    public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// Gets or sets the HTTP response content.
    /// </summary>
    public string Content { get; set; }
}
