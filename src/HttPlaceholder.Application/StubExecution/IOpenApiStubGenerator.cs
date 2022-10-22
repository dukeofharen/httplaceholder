using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution;

/// <summary>
///     Describes a class that is being used to convert an OpenAPI definition into stubs.
/// </summary>
public interface IOpenApiStubGenerator
{
    /// <summary>
    ///     Converts an OpenAPI definition into stubs.
    /// </summary>
    /// <param name="input">The OpenAPI JSON or YAML definition.</param>
    /// <param name="doNotCreateStub">
    ///     Whether to add the stub to the data source. If set to false, the stub is only returned
    ///     but not added.
    /// </param>
    /// <param name="tenant">The tenant the stubs should be created under.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of created stubs.</returns>
    Task<IEnumerable<FullStubModel>> GenerateOpenApiStubsAsync(string input, bool doNotCreateStub, string tenant,
        CancellationToken cancellationToken);
}
