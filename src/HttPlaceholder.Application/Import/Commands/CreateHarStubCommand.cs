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
public class CreateHarStubCommandHandler : IRequestHandler<CreateHarStubCommand, IEnumerable<FullStubModel>>
{
    private readonly IHarStubGenerator _harStubGenerator;
    private readonly IDateTime _dateTime;

    /// <summary>
    ///     Constructs a <see cref="CreateHarStubCommandHandler" /> instance.
    /// </summary>
    public CreateHarStubCommandHandler(IHarStubGenerator harStubGenerator, IDateTime dateTime)
    {
        _harStubGenerator = harStubGenerator;
        _dateTime = dateTime;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FullStubModel>> Handle(CreateHarStubCommand request,
        CancellationToken cancellationToken)
    {
        var tenant = string.IsNullOrWhiteSpace(request.Tenant)
            ? $"har-import-{_dateTime.Now:yyyy-MM-dd-HH-mm-ss}"
            : request.Tenant;
        return await _harStubGenerator.GenerateStubsAsync(request.Input, request.DoNotCreateStub, tenant,
            request.StubIdPrefix,
            cancellationToken);
    }
}
