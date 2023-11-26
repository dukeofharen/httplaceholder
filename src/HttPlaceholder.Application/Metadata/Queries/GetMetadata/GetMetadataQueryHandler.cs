using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Application.Metadata.Queries.GetMetadata;

/// <summary>
///     A query handler for retrieving metadata about the currently running instance of HttPlaceholder.
/// </summary>
public class GetMetadataQueryHandler : IRequestHandler<GetMetadataQuery, MetadataModel>
{
    private readonly IAssemblyService _assemblyService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IEnvService _envService;

    /// <summary>
    ///     Constructs a <see cref="GetMetadataQueryHandler" /> instance.
    /// </summary>
    public GetMetadataQueryHandler(
        IAssemblyService assemblyService,
        IServiceProvider serviceProvider,
        IEnvService envService)
    {
        _assemblyService = assemblyService;
        _serviceProvider = serviceProvider;
        _envService = envService;
    }

    /// <inheritdoc />
    public Task<MetadataModel> Handle(GetMetadataQuery request, CancellationToken cancellationToken)
    {
        var handlers = _serviceProvider.GetServices<IResponseVariableParsingHandler>();
        var result = new MetadataModel
        {
            Version = _assemblyService.GetAssemblyVersion(),
            VariableHandlers = handlers.Select(h =>
                new VariableHandlerModel(h.Name, h.FullName, h.Examples, h.GetDescription())),
            RuntimeVersion = _envService.GetRuntime()
        };
        return Task.FromResult(result);
    }
}
