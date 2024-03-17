using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Import.Commands;

/// <summary>
///     A command for creating stubs based on cURL commands.
/// </summary>
public class CreateCurlStubCommand(string input, bool doNotCreateStub, string tenant, string stubIdPrefix) : IRequest<IEnumerable<FullStubModel>>, IImportStubsCommand
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
///     A command handler for creating stubs based on cURL commands.
/// </summary>
public class CreateCurlStubCommandHandler : IRequestHandler<CreateCurlStubCommand, IEnumerable<FullStubModel>>
{
    private readonly ICurlStubGenerator _curlStubGenerator;
    private readonly IDateTime _dateTime;

    /// <summary>
    ///     Constructs a <see cref="CreateCurlStubCommandHandler" /> instance.
    /// </summary>
    public CreateCurlStubCommandHandler(ICurlStubGenerator curlStubGenerator, IDateTime dateTime)
    {
        _curlStubGenerator = curlStubGenerator;
        _dateTime = dateTime;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FullStubModel>> Handle(CreateCurlStubCommand request,
        CancellationToken cancellationToken)
    {
        var tenant = string.IsNullOrWhiteSpace(request.Tenant)
            ? $"curl-import-{_dateTime.Now:yyyy-MM-dd-HH-mm-ss}"
            : request.Tenant;
        return await _curlStubGenerator.GenerateStubsAsync(request.Input, request.DoNotCreateStub, tenant,
            request.StubIdPrefix,
            cancellationToken);
    }
}
