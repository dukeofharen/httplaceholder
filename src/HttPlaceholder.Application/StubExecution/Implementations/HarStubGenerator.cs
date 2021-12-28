using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.Implementations;

/// <inheritdoc />
public class HarStubGenerator : IHarStubGenerator
{
    /// <inheritdoc />
    public Task<IEnumerable<FullStubModel>> GenerateHarStubsAsync(string input, bool doNotCreateStub) => throw new System.NotImplementedException();
}
