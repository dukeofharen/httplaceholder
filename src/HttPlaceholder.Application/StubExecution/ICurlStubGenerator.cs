using System.Collections.Generic;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution
{
    /// <summary>
    /// Describes a class that is being used to convert one or several cURL commands into stubs.
    /// </summary>
    public interface ICurlStubGenerator
    {
        /// <summary>
        /// Converts one or several cURL commands into stubs.
        /// </summary>
        /// <param name="input">The cURL command(s).</param>
        /// <param name="doNotCreateStub">Whether to add the stub to the data source. If set to false, the stub is only returned but not added.</param>
        /// <returns>A list of created stubs.</returns>
        IEnumerable<FullStubModel> GenerateCurlStubs(string input, bool doNotCreateStub);
    }
}
