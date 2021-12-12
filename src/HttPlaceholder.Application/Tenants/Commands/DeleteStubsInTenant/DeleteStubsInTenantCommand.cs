using MediatR;

namespace HttPlaceholder.Application.Tenants.Commands.DeleteStubsInTenant;

public class DeleteStubsInTenantCommand : IRequest
{
    public DeleteStubsInTenantCommand(string tenant)
    {
        Tenant = tenant;
    }

    public string Tenant { get; }
}