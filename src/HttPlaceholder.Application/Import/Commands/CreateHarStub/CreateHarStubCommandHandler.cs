using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Import.Commands.CreateHarStub;

/// <summary>
///     A command handler for creating stubs based on HTTP archive (HAR).
/// </summary>
public class CreateHarStubCommandHandler : IRequestHandler<CreateHarStubCommand, IEnumerable<FullStubModel>>
{
    private readonly IHarStubGenerator _harStubGenerator;

    /// <summary>
    ///     Constructs a <see cref="CreateHarStubCommandHandler" /> instance.
    /// </summary>
    /// <param name="harStubGenerator"></param>
    public CreateHarStubCommandHandler(IHarStubGenerator harStubGenerator)
    {
        _harStubGenerator = harStubGenerator;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FullStubModel>> Handle(CreateHarStubCommand request,
        CancellationToken cancellationToken)
    {
        var tenant = string.IsNullOrWhiteSpace(request.Tenant)
            ? $"har-import-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}"
            : request.Tenant;
        return await _harStubGenerator.GenerateStubsAsync(request.Input, request.DoNotCreateStub, tenant,
            request.StubIdPrefix,
            cancellationToken);
    }
}
