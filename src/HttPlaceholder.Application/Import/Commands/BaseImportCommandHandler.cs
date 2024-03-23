using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Import.Commands;

/// <summary>
///     An abstract base class for importing several inputs to stubs.
/// </summary>
public abstract class BaseImportCommandHandler(
    IStubGenerator stubGenerator,
    IDateTime dateTime) : IRequestHandler<IImportStubsCommand, IEnumerable<FullStubModel>>
{
    /// <summary>
    ///     Returns the tenant prefix.
    /// </summary>
    /// <returns>The tenant prefix.</returns>
    protected abstract string GetTenantPrefix();

    /// <inheritdoc/>
    public async Task<IEnumerable<FullStubModel>> Handle(IImportStubsCommand request,
        CancellationToken cancellationToken)
    {
        var tenant = string.IsNullOrWhiteSpace(request.Tenant)
            ? $"{GetTenantPrefix()}-{dateTime.Now:yyyy-MM-dd-HH-mm-ss}"
            : request.Tenant;
        return await stubGenerator.GenerateStubsAsync(
            request.Input,
            request.DoNotCreateStub,
            tenant,
            request.StubIdPrefix,
            cancellationToken);
    }
}
