using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using MediatR;

namespace HttPlaceholder.Application.Tenants.Commands;

/// <summary>
///     A command for deleting all stubs belonging to a specific tenant.
/// </summary>
public class DeleteStubsInTenantCommand(string tenant) : IRequest<Unit>
{
    /// <summary>
    ///     Gets the tenant.
    /// </summary>
    public string Tenant { get; } = tenant;
}

/// <summary>
///     A command handler for deleting all stubs belonging to a specific tenant.
/// </summary>
public class DeleteStubsInTenantCommandHandler(IStubContext stubContext)
    : IRequestHandler<DeleteStubsInTenantCommand, Unit>
{
    /// <inheritdoc />
    public async Task<Unit> Handle(DeleteStubsInTenantCommand request, CancellationToken cancellationToken)
    {
        await stubContext.DeleteAllStubsAsync(request.Tenant, cancellationToken);
        return Unit.Value;
    }
}
