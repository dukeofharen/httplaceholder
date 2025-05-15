using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Commands;

/// <summary>
///     A command for creating a stub from a request.
/// </summary>
public class CreateStubForRequestCommand(string correlationId, bool doNotCreateStub) : IRequest<FullStubModel>
{
    /// <summary>
    ///     Gets the request correlation ID.
    /// </summary>
    public string CorrelationId { get; } = correlationId;

    /// <summary>
    ///     Gets whether to create the stub or not.
    /// </summary>
    public bool DoNotCreateStub { get; } = doNotCreateStub;
}

/// <summary>
///     A command handler for creating a stub from a request.
/// </summary>
public class CreateStubForRequestCommandHandler(IRequestStubGenerator requestStubGenerator)
    : IRequestHandler<CreateStubForRequestCommand, FullStubModel>
{
    /// <inheritdoc />
    public async Task<FullStubModel> Handle(
        CreateStubForRequestCommand request,
        CancellationToken cancellationToken) =>
        await requestStubGenerator.GenerateStubBasedOnRequestAsync(request.CorrelationId, request.DoNotCreateStub,
            cancellationToken);
}
