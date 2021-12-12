using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Import.Commands;

public class CreateCurlStubCommandHandler : IRequestHandler<CreateCurlStubCommand, IEnumerable<FullStubModel>>
{
    private readonly ICurlStubGenerator _curlStubGenerator;

    public CreateCurlStubCommandHandler(ICurlStubGenerator curlStubGenerator)
    {
        _curlStubGenerator = curlStubGenerator;
    }

    public async Task<IEnumerable<FullStubModel>> Handle(CreateCurlStubCommand request,
        CancellationToken cancellationToken) =>
        await _curlStubGenerator.GenerateCurlStubsAsync(request.CurlCommand, request.DoNotCreateStub);
}