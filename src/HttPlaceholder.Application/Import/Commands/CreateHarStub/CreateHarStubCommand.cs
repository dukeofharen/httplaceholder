using System.Collections.Generic;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Import.Commands.CreateHarStub;

/// <summary>
/// A command for creating stubs based on HTTP archive (HAR).
/// </summary>
public class CreateHarStubCommand : IRequest<IEnumerable<FullStubModel>>
{
    /// <summary>
    /// Constructs a <see cref="CreateHarStubCommand"/> instance.
    /// </summary>
    /// <param name="har">The HAR.</param>
    /// <param name="doNotCreateStub">Whether or not to create stubs.</param>
    /// <param name="tenant">The stub tenant.</param>
    public CreateHarStubCommand(string har, bool doNotCreateStub, string tenant)
    {
        Har = har;
        DoNotCreateStub = doNotCreateStub;
        Tenant = tenant;
    }

    /// <summary>
    /// Gets the HTTP archive.
    /// </summary>
    public string Har { get; }

    /// <summary>
    /// Gets whether to create stubs or not.
    /// </summary>
    public bool DoNotCreateStub { get; }

    /// <summary>
    /// Gets the stub tenant.
    /// </summary>
    public string Tenant { get; }
}
