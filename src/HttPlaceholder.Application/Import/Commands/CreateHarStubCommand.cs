using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Import.Commands;

/// <summary>
///     A command for creating stubs based on HTTP archive (HAR).
/// </summary>
public class CreateHarStubCommand(string input, bool doNotCreateStub, string tenant, string stubIdPrefix)
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
///     A command handler for creating stubs based on HTTP archive (HAR).
/// </summary>
public class CreateHarStubCommandHandler(IHarStubGenerator harStubGenerator, IDateTime dateTime) : IRequestHandler<CreateHarStubCommand, IEnumerable<FullStubModel>>
{
    /// <inheritdoc />
    public async Task<IEnumerable<FullStubModel>> Handle(CreateHarStubCommand request,
        CancellationToken cancellationToken)
    {
        var tenant = string.IsNullOrWhiteSpace(request.Tenant)
            ? $"har-import-{dateTime.Now:yyyy-MM-dd-HH-mm-ss}"
            : request.Tenant;
        return await harStubGenerator.GenerateStubsAsync(request.Input, request.DoNotCreateStub, tenant,
            request.StubIdPrefix,
            cancellationToken);
    }
}
