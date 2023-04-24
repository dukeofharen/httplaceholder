using System.Collections.Generic;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Import.Commands.CreateCurlStub;

/// <summary>
///     A command for creating stubs based on cURL commands.
/// </summary>
public class CreateCurlStubCommand : IRequest<IEnumerable<FullStubModel>>, IImportStubsCommand
{
    /// <summary>
    ///     Constructs a <see cref="CreateCurlStubCommand" /> instance.
    /// </summary>
    /// <param name="input">The import input.</param>
    /// <param name="doNotCreateStub">Whether or not to create stubs.</param>
    /// <param name="tenant">The stub tenant.</param>
    /// <param name="stubIdPrefix">A prefix that is put before the stub ID.</param>
    public CreateCurlStubCommand(string input, bool doNotCreateStub, string tenant, string stubIdPrefix)
    {
        Input = input;
        DoNotCreateStub = doNotCreateStub;
        Tenant = tenant;
        StubIdPrefix = stubIdPrefix;
    }

    /// <inheritdoc />
    public string Input { get; }

    /// <inheritdoc />
    public bool DoNotCreateStub { get; }

    /// <inheritdoc />
    public string Tenant { get; }

    /// <inheritdoc />
    public string StubIdPrefix { get; }
}
