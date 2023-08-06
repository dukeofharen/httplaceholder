using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Commands.UpdateStubCommand;

/// <summary>
///     A command for updating a stub.
/// </summary>
public class UpdateStubCommand : IRequest<Unit>
{
    /// <summary>
    ///     Constructs an <see cref="UpdateStubCommand" /> instance.
    /// </summary>
    /// <param name="stubId">The stub ID to update.</param>
    /// <param name="stub">The stub to update.</param>
    public UpdateStubCommand(string stubId, StubModel stub)
    {
        StubId = stubId;
        Stub = stub;
    }

    /// <summary>
    ///     Gets the stub ID to update.
    /// </summary>
    public string StubId { get; }

    /// <summary>
    ///     Gets the stub to update.
    /// </summary>
    public StubModel Stub { get; }
}
