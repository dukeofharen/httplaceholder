using MediatR;

namespace HttPlaceholder.Application.Tenants.Commands.DeleteStubsInTenant
{
    public class DeleteStubsInTenantCommand : IRequest
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Tenant { get; set; }
    }
}
