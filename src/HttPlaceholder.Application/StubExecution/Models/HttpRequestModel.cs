using System.Collections.Generic;
using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.Models;

/// <summary>
/// A model that contains
/// </summary>
public class HttpRequestModel : IMapFrom<RequestParametersModel>
{
    /// <summary>
    /// Gets or sets the method.
    /// </summary>
    public string Method { get; set; }

    /// <summary>
    /// Gets or sets the URL.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Gets or sets the body.
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    /// Gets or sets the headers.
    /// </summary>
    public IDictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// Gets or sets the client ip.
    /// </summary>
    public string ClientIp { get; set; }
}