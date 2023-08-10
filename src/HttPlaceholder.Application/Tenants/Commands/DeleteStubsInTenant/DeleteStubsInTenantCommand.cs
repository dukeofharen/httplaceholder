using MediatR;

namespace HttPlaceholder.Application.Tenants.Commands.DeleteStubsInTenant;

/// <summary>
///     A command for deleting all stubs belonging to a specific tenant.
/// </summary>
public class DeleteStubsInTenantCommand : IRequest<Unit>
{
    /// <summary>
    ///     Constructs a <see cref="DeleteStubsInTenantCommand" /> instance.
    /// </summary>
    /// <param name="tenant">The tenant.</param>
    public DeleteStubsInTenantCommand(string tenant)
    {
        Tenant = tenant;
    }

    /// <summary>
    ///     Gets the tenant.
    /// </summary>
    public string Tenant { get; }
}
