using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Import.Commands.CreateHarStub;

public class CreateHarStubCommandHandler : IRequestHandler<CreateHarStubCommand, IEnumerable<FullStubModel>>
{
    private readonly IHarStubGenerator _harStubGenerator;

    public CreateHarStubCommandHandler(IHarStubGenerator harStubGenerator)
    {
        _harStubGenerator = harStubGenerator;
    }

    public async Task<IEnumerable<FullStubModel>> Handle(CreateHarStubCommand request,
        CancellationToken cancellationToken) =>
        await _harStubGenerator.GenerateHarStubsAsync(request.Har, request.DoNotCreateStub);
}
