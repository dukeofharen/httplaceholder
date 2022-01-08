using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution;

/// <summary>
/// Describes a class that is being used to convert an HTTP Archive (HAR) into stubs.
/// </summary>
public interface IHarStubGenerator
{
    /// <summary>
    /// Converts an HTTP Archive (HAR) into stubs.
    /// </summary>
    /// <param name="input">The cURL command(s).</param>
    /// <param name="doNotCreateStub">Whether to add the stub to the data source. If set to false, the stub is only returned but not added.</param>
    /// <param name="tenant">The tenant the stubs should be created under.</param>
    /// <returns>A list of created stubs.</returns>
    Task<IEnumerable<FullStubModel>> GenerateHarStubsAsync(string input, bool doNotCreateStub, string tenant);
}
