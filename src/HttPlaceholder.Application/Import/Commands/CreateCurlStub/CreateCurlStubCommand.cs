using System.Collections.Generic;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Import.Commands.CreateCurlStub;

public class CreateCurlStubCommand : IRequest<IEnumerable<FullStubModel>>
{
    public CreateCurlStubCommand(string curlCommand, bool doNotCreateStub)
    {
        CurlCommand = curlCommand;
        DoNotCreateStub = doNotCreateStub;
    }

    public string CurlCommand { get; }

    public bool DoNotCreateStub { get; }
}
