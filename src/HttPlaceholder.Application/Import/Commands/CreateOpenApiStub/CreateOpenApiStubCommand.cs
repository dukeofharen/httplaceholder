using System.Collections.Generic;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Import.Commands.CreateOpenApiStub;

public class CreateOpenApiStubCommand : IRequest<IEnumerable<FullStubModel>>
{
    public CreateOpenApiStubCommand(string openApi, bool doNotCreateStub)
    {
        OpenApi = openApi;
        DoNotCreateStub = doNotCreateStub;
    }

    public string OpenApi { get; }

    public bool DoNotCreateStub { get; }
}
