using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Common;

namespace HttPlaceholder.Application.Import.Commands;

/// <summary>
///     A command for creating stubs based on OpenAPI definitions.
/// </summary>
public class CreateOpenApiStubCommand(string input, bool doNotCreateStub, string tenant, string stubIdPrefix)
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
///     A command handler for creating stubs based on OpenAPI definitions.
/// </summary>
public class CreateOpenApiStubCommandHandler(IOpenApiStubGenerator openApiStubGenerator, IDateTime dateTime)
    : BaseImportCommandHandler(openApiStubGenerator, dateTime)
{
    /// <inheritdoc/>
    protected override string GetTenantPrefix() => "openapi-import";
}
