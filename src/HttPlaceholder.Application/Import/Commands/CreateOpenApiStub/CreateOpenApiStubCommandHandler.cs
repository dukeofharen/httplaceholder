using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Import.Commands.CreateOpenApiStub;

/// <summary>
///     A command handler for creating stubs based on OpenAPI definitions.
/// </summary>
public class CreateOpenApiStubCommandHandler : IRequestHandler<CreateOpenApiStubCommand, IEnumerable<FullStubModel>>
{
    private readonly IOpenApiStubGenerator _openApiStubGenerator;

    /// <summary>
    ///     Constructs a <see cref="CreateOpenApiStubCommandHandler" /> instance.
    /// </summary>
    public CreateOpenApiStubCommandHandler(IOpenApiStubGenerator openApiStubGenerator)
    {
        _openApiStubGenerator = openApiStubGenerator;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FullStubModel>> Handle(
        CreateOpenApiStubCommand request,
        CancellationToken cancellationToken)
    {
        var tenant = string.IsNullOrWhiteSpace(request.Tenant)
            ? $"openapi-import-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}"
            : request.Tenant;
        return await _openApiStubGenerator.GenerateStubsAsync(request.Input, request.DoNotCreateStub, tenant,
            request.StubIdPrefix,
            cancellationToken);
    }
}
