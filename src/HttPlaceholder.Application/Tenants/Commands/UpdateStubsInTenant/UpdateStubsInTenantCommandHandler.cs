using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using MediatR;

namespace HttPlaceholder.Application.Tenants.Commands.UpdateStubsInTenant;

/// <summary>
///     A command handler for updating all stubs belonging to a tenant.
/// </summary>
public class UpdateStubsInTenantCommandHandler : IRequestHandler<UpdateStubsInTenantCommand, Unit>
{
    private readonly IStubContext _stubContext;

    /// <summary>
    ///     Constructs an <see cref="UpdateStubsInTenantCommandHandler" /> instance.
    /// </summary>
    public UpdateStubsInTenantCommandHandler(IStubContext stubContext)
    {
        _stubContext = stubContext;
    }

    /// <inheritdoc />
    public async Task<Unit> Handle(UpdateStubsInTenantCommand request, CancellationToken cancellationToken)
    {
        await _stubContext.UpdateAllStubs(request.Tenant, request.Stubs, cancellationToken);
        return Unit.Value;
    }
}
