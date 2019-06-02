using MediatR;

namespace HttPlaceholder.Application.Tenants.Commands.DeleteStubsInTenant
{
    public class DeleteStubsInTenantCommand : IRequest
    {
        public string Tenant { get; set; }
    }
}
