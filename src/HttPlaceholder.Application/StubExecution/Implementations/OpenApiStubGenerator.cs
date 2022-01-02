using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.Implementations;

/// <inheritdoc />
public class OpenApiStubGenerator : IOpenApiStubGenerator
{
    /// <inheritdoc />
    public Task<IEnumerable<FullStubModel>> GenerateOpenApiStubs(string input, bool doNotCreateStub) =>
        throw new System.NotImplementedException();
}
