using System.Collections.Generic;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Commands.AddStubs;

/// <summary>
/// A command for adding multiple stubs.
/// </summary>
public class AddStubsCommand : IRequest<IEnumerable<FullStubModel>>
{
    /// <summary>
    /// Constructs a <see cref="AddStubsCommand"/> instance.
    /// </summary>
    /// <param name="stubs">The stubs to add.</param>
    public AddStubsCommand(IEnumerable<StubModel> stubs)
    {
        Stubs = stubs;
    }

    /// <summary>
    /// Gets the stubs to add.
    /// </summary>
    public IEnumerable<StubModel> Stubs { get; }
}
