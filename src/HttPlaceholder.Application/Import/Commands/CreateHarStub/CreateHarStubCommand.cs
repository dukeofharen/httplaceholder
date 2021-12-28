using System.Collections.Generic;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Import.Commands.CreateHarStub;

public class CreateHarStubCommand : IRequest<IEnumerable<FullStubModel>>
{
    public CreateHarStubCommand(string har, bool doNotCreateStub)
    {
        Har = har;
        DoNotCreateStub = doNotCreateStub;
    }

    public string Har { get; }

    public bool DoNotCreateStub { get; }
}
