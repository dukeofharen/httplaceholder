using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Common;

namespace HttPlaceholder.Application.Import.Commands;

/// <summary>
///     A command for creating stubs based on cURL commands.
/// </summary>
public class CreateCurlStubCommand(string input, bool doNotCreateStub, string tenant, string stubIdPrefix)
    : IImportStubsCommand
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
public class CreateCurlStubCommandHandler(ICurlStubGenerator curlStubGenerator, IDateTime dateTime)
    : BaseImportCommandHandler<CreateCurlStubCommand>(curlStubGenerator, dateTime)
{
    /// <inheritdoc/>
    protected override string GetTenantPrefix() => "curl-import";
}
