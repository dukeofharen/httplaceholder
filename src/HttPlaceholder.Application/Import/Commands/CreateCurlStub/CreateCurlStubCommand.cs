using System.Collections.Generic;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Import.Commands.CreateCurlStub;

/// <summary>
///     A command for creating stubs based on cURL commands.
/// </summary>
public class CreateCurlStubCommand : IRequest<IEnumerable<FullStubModel>>
{
    /// <summary>
    ///     Constructs a <see cref="CreateCurlStubCommand" /> instance.
    /// </summary>
    /// <param name="curlCommand">The cURL commands.</param>
    /// <param name="doNotCreateStub">Whether or not to create stubs.</param>
    /// <param name="tenant">The stub tenant.</param>
    public CreateCurlStubCommand(string curlCommand, bool doNotCreateStub, string tenant)
    {
        CurlCommand = curlCommand;
        DoNotCreateStub = doNotCreateStub;
        Tenant = tenant;
    }

    /// <summary>
    ///     Gets the cURL commands.
    /// </summary>
    public string CurlCommand { get; }

    /// <summary>
    ///     Gets whether to create stubs or not.
    /// </summary>
    public bool DoNotCreateStub { get; }

    /// <summary>
    ///     Gets the stub tenant.
    /// </summary>
    public string Tenant { get; }
}
