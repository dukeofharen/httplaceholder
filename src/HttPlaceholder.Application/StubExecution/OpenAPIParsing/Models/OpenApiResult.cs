using System.Collections.Generic;

namespace HttPlaceholder.Application.StubExecution.OpenAPIParsing.Models;

/// <summary>
/// Describes the parsed OpenAPI result. Contains metadata about the API and definitions where stubs should be created for.
/// </summary>
public class OpenApiResult
{
    /// <summary>
    /// Gets or sets the OpenAPI server URL.
    /// </summary>
    public string ServerUrl { get; set; }

    /// <summary>
    /// Gets or sets the OpenAPI definitions.
    /// </summary>
    public IEnumerable<OpenApiLine> Lines { get; set; }
}
