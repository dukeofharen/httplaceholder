using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Commands.CreateStubForRequest;

/// <summary>
///     A command for creating a stub from a request.
/// </summary>
public class CreateStubForRequestCommand : IRequest<FullStubModel>
{
    /// <summary>
    ///     Constructs a <see cref="CreateStubForRequestCommand" /> instance.
    /// </summary>
    /// <param name="correlationId">The request correlation ID.</param>
    /// <param name="doNotCreateStub">Whether to create the stub or not.</param>
    public CreateStubForRequestCommand(string correlationId, bool doNotCreateStub)
    {
        CorrelationId = correlationId;
        DoNotCreateStub = doNotCreateStub;
    }

    /// <summary>
    ///     Gets the request correlation ID.
    /// </summary>
    public string CorrelationId { get; }

    /// <summary>
    ///     Gets whether to create the stub or not.
    /// </summary>
    public bool DoNotCreateStub { get; }
}
