using Microsoft.OpenApi.Models;

namespace HttPlaceholder.Application.StubExecution.OpenAPIParsing.Models;

/// <summary>
///     Describes an Open API definition. One of these models equals one stub to be created.
/// </summary>
public class OpenApiLine
{
    /// <summary>
    ///     Gets or sets the OpenAPI operation.
    /// </summary>
    public OpenApiOperation Operation { get; set; }

    /// <summary>
    ///     Gets or sets the OpenAPI operation type.
    /// </summary>
    public OperationType OperationType { get; set; }

    /// <summary>
    ///     Gets or sets the OpenAPI path key.
    /// </summary>
    public string PathKey { get; set; }

    /// <summary>
    ///     Gets or sets the OpenAPI response.
    /// </summary>
    public OpenApiResponse Response { get; set; }

    /// <summary>
    ///     Gets or sets the OpenAPI response key.
    /// </summary>
    public string ResponseKey { get; set; }
}
