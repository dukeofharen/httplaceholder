using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Commands.CreateStubForRequest;

/// <summary>
/// A command handler for creating a stub from a request.
/// </summary>
public class CreateStubForRequestCommandHandler : IRequestHandler<CreateStubForRequestCommand, FullStubModel>
{
    private readonly IRequestStubGenerator _requestStubGenerator;

    /// <summary>
    /// Constructs a <see cref="CreateStubForRequestCommandHandler"/> instance.
    /// </summary>
    public CreateStubForRequestCommandHandler(IRequestStubGenerator requestStubGenerator)
    {
        _requestStubGenerator = requestStubGenerator;
    }

    /// <inheritdoc />
    public async Task<FullStubModel> Handle(
        CreateStubForRequestCommand request,
        CancellationToken cancellationToken) =>
        await _requestStubGenerator.GenerateStubBasedOnRequestAsync(request.CorrelationId, request.DoNotCreateStub);
}
