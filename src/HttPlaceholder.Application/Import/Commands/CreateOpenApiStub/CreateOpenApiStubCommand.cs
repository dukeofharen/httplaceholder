using System.Collections.Generic;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Import.Commands.CreateOpenApiStub;

/// <summary>
///     A command for creating stubs based on OpenAPI definitions.
/// </summary>
public class CreateOpenApiStubCommand : IRequest<IEnumerable<FullStubModel>>
{
    /// <summary>
    ///     Constructs a <see cref="CreateOpenApiStubCommand" /> instance.
    /// </summary>
    /// <param name="openApi">The OpenAPI definition.</param>
    /// <param name="doNotCreateStub">Whether or not to create stubs.</param>
    /// <param name="tenant">The stub tenant.</param>
    public CreateOpenApiStubCommand(string openApi, bool doNotCreateStub, string tenant)
    {
        OpenApi = openApi;
        DoNotCreateStub = doNotCreateStub;
        Tenant = tenant;
    }

    /// <summary>
    ///     Gets the OpenAPI definition.
    /// </summary>
    public string OpenApi { get; }

    /// <summary>
    ///     Gets whether to create stubs or not.
    /// </summary>
    public bool DoNotCreateStub { get; }

    /// <summary>
    ///     Gets the stub tenant.
    /// </summary>
    public string Tenant { get; }
}
