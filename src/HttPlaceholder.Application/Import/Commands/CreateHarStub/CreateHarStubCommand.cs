using System.Collections.Generic;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Import.Commands.CreateHarStub;

/// <summary>
///     A command for creating stubs based on HTTP archive (HAR).
/// </summary>
public class CreateHarStubCommand : IRequest<IEnumerable<FullStubModel>>, IImportStubsCommand
{
    /// <summary>
    ///     Constructs a <see cref="CreateHarStubCommand" /> instance.
    /// </summary>
    /// <param name="input">The import input.</param>
    /// <param name="doNotCreateStub">Whether or not to create stubs.</param>
    /// <param name="tenant">The stub tenant.</param>
    /// <param name="stubIdPrefix">A prefix that is put before the stub ID.</param>
    public CreateHarStubCommand(string input, bool doNotCreateStub, string tenant, string stubIdPrefix)
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
