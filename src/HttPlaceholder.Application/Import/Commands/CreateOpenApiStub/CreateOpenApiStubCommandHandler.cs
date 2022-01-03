using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Import.Commands.CreateOpenApiStub;

public class CreateOpenApiStubCommandHandler : IRequestHandler<CreateOpenApiStubCommand, IEnumerable<FullStubModel>>
{
    private readonly IOpenApiStubGenerator _openApiStubGenerator;

    public CreateOpenApiStubCommandHandler(IOpenApiStubGenerator openApiStubGenerator)
    {
        _openApiStubGenerator = openApiStubGenerator;
    }

    public async Task<IEnumerable<FullStubModel>> Handle(
        CreateOpenApiStubCommand request,
        CancellationToken cancellationToken) =>
        await _openApiStubGenerator.GenerateOpenApiStubsAsync(request.OpenApi, request.DoNotCreateStub);
}
