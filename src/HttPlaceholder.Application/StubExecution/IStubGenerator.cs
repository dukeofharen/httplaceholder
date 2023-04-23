using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution;

/// <summary>
///     Describes a class that is used to generate stubs based on a specific input
/// </summary>
public interface IStubGenerator
{
    /// <summary>
    ///     Converts an input into stubs.
    /// </summary>
    /// <param name="input">The input for which to generate stubs.</param>
    /// <param name="doNotCreateStub">
    ///     Whether to add the stub to the data source. If set to false, the stub is only returned
    ///     but not added.
    /// </param>
    /// <param name="tenant">The tenant the stubs should be created under.</param>
    /// <param name="stubIdPrefix">A piece of text that will be prefixed before the stub ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of created stubs.</returns>
    Task<IEnumerable<FullStubModel>> GenerateStubsAsync(string input, bool doNotCreateStub, string tenant,
        string stubIdPrefix,
        CancellationToken cancellationToken);
}
