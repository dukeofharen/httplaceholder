using MediatR;

namespace HttPlaceholder.Application.Tenants.Commands.DeleteStubsInTenant
{
    public class DeleteStubsInTenantCommand : IRequest
    {
        public DeleteStubsInTenantCommand(string tenant)
        {
            Tenant = tenant;
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Tenant { get; }
    }
}
