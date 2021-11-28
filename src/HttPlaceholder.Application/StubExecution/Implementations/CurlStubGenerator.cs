using System.Collections.Generic;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.Implementations
{
    /// <inheritdoc />
    internal class CurlStubGenerator : ICurlStubGenerator
    {
        /// <inheritdoc />
        public IEnumerable<FullStubModel> GenerateCurlStubs(string input, bool doNotCreateStub) => throw new System.NotImplementedException();
    }
}
