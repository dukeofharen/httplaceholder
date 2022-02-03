using System.Collections.Generic;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Tenants.Commands.UpdateStubsInTenant;

/// <summary>
/// A command for updating all stubs belonging to a tenant.
/// </summary>
public class UpdateStubsInTenantCommand : IRequest
{
    /// <summary>
    /// Constructs an <see cref="UpdateStubsInTenantCommand"/> instance.
    /// </summary>
    /// <param name="stubs">The stubs that should be updated.</param>
    /// <param name="tenant">The tenant.</param>
    public UpdateStubsInTenantCommand(IEnumerable<StubModel> stubs, string tenant)
    {
        Stubs = stubs;
        Tenant = tenant;
    }

    /// <summary>
    /// Gets the stubs that should be updated.
    /// </summary>
    public IEnumerable<StubModel> Stubs { get; }

    /// <summary>
    /// Gets the tenant.
    /// </summary>
    public string Tenant { get; }
}
