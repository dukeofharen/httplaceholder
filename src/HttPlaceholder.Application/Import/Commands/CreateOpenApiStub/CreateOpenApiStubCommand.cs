using System.Collections.Generic;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Import.Commands.CreateOpenApiStub;

public class CreateOpenApiStubCommand : IRequest<IEnumerable<FullStubModel>>
{
    public CreateOpenApiStubCommand(string openApi, bool doNotCreateStub, string tenant)
    {
        OpenApi = openApi;
        DoNotCreateStub = doNotCreateStub;
        Tenant = tenant;
    }

    public string OpenApi { get; }

    public bool DoNotCreateStub { get; }

    public string Tenant { get; }
}
