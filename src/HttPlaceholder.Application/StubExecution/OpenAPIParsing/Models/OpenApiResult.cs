using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace HttPlaceholder.Application.StubExecution.OpenAPIParsing.Models;

/// <summary>
///     Describes the parsed OpenAPI result. Contains metadata about the API and definitions where stubs should be created
///     for.
/// </summary>
public class OpenApiResult
{
    /// <summary>
    ///     Gets or sets the OpenAPI server.
    /// </summary>
    public OpenApiServer Server { get; set; }

    /// <summary>
    ///     Gets or sets the OpenAPI definitions.
    /// </summary>
    public IEnumerable<OpenApiLine> Lines { get; set; }
}
