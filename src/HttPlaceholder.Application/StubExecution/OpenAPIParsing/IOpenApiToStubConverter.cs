using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.OpenAPIParsing.Models;
using HttPlaceholder.Domain;
using Microsoft.OpenApi.Models;

namespace HttPlaceholder.Application.StubExecution.OpenAPIParsing;

/// <summary>
/// Describes a class that is used to convert a single OpenAPI definition into an HttPlaceholder stub.
/// </summary>
public interface IOpenApiToStubConverter
{
    /// <summary>
    /// Converts a single OpenAPI definition into a <see cref="StubModel"/>.
    /// </summary>
    /// <param name="server">The server OpenAPI server.</param>
    /// <param name="line">The <see cref="OpenApiLine"/> (definition).</param>
    /// <param name="tenant">The tenant the stubs should be created under.</param>
    /// <returns>The converted <see cref="StubModel"/>.</returns>
    Task<StubModel> ConvertToStubAsync(OpenApiServer server, OpenApiLine line, string tenant);
}
