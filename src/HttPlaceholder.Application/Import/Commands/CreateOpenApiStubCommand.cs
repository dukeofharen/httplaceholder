using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Import.Commands;

/// <summary>
///     A command for creating stubs based on OpenAPI definitions.
/// </summary>
public class CreateOpenApiStubCommand(string input, bool doNotCreateStub, string tenant, string stubIdPrefix)
    : IRequest<IEnumerable<FullStubModel>>, IImportStubsCommand
{
    /// <inheritdoc />
    public string Input { get; } = input;

    /// <inheritdoc />
    public bool DoNotCreateStub { get; } = doNotCreateStub;

    /// <inheritdoc />
    public string Tenant { get; } = tenant;

    /// <inheritdoc />
    public string StubIdPrefix { get; } = stubIdPrefix;
}

/// <summary>
///     A command handler for creating stubs based on OpenAPI definitions.
/// </summary>
public class CreateOpenApiStubCommandHandler : IRequestHandler<CreateOpenApiStubCommand, IEnumerable<FullStubModel>>
{
    private readonly IOpenApiStubGenerator _openApiStubGenerator;
    private readonly IDateTime _dateTime;

    /// <summary>
    ///     Constructs a <see cref="CreateOpenApiStubCommandHandler" /> instance.
    /// </summary>
    public CreateOpenApiStubCommandHandler(IOpenApiStubGenerator openApiStubGenerator, IDateTime dateTime)
    {
        _openApiStubGenerator = openApiStubGenerator;
        _dateTime = dateTime;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FullStubModel>> Handle(
        CreateOpenApiStubCommand request,
        CancellationToken cancellationToken)
    {
        var tenant = string.IsNullOrWhiteSpace(request.Tenant)
            ? $"openapi-import-{_dateTime.Now:yyyy-MM-dd-HH-mm-ss}"
            : request.Tenant;
        return await _openApiStubGenerator.GenerateStubsAsync(request.Input, request.DoNotCreateStub, tenant,
            request.StubIdPrefix,
            cancellationToken);
    }
}
