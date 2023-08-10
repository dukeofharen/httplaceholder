using MediatR;

namespace HttPlaceholder.Application.Stubs.Commands.DeleteStub;

/// <summary>
///     A command for deleting a stub.
/// </summary>
public class DeleteStubCommand : IRequest<Unit>
{
    /// <summary>
    ///     Constructs a <see cref="DeleteStubCommand" /> instance.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    public DeleteStubCommand(string stubId)
    {
        StubId = stubId;
    }

    /// <summary>
    ///     Gets the stub ID.
    /// </summary>
    public string StubId { get; }
}
