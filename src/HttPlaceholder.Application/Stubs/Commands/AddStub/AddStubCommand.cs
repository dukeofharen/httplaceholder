using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Commands.AddStub;

/// <summary>
/// A command for adding a stub.
/// </summary>
public class AddStubCommand : IRequest<FullStubModel>
{
    /// <summary>
    /// Constructs a <see cref="AddStubCommand"/> instance.
    /// </summary>
    /// <param name="stub">The stub to add.</param>
    public AddStubCommand(StubModel stub)
    {
        Stub = stub;
    }

    /// <summary>
    /// Gets the stub to add.
    /// </summary>
    public StubModel Stub { get; }
}
