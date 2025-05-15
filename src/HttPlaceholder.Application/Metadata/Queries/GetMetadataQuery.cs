using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Application.Metadata.Queries;

/// <summary>
///     A query for retrieving metadata about the currently running instance of HttPlaceholder.
/// </summary>
public class GetMetadataQuery : IRequest<MetadataModel>;

/// <summary>
///     A query handler for retrieving metadata about the currently running instance of HttPlaceholder.
/// </summary>
public class GetMetadataQueryHandler(
    IAssemblyService assemblyService,
    IServiceProvider serviceProvider,
    IEnvService envService) : IRequestHandler<GetMetadataQuery, MetadataModel>
{
    /// <inheritdoc />
    public Task<MetadataModel> Handle(GetMetadataQuery request, CancellationToken cancellationToken) =>
        new MetadataModel
        {
            Version = assemblyService.GetAssemblyVersion(),
            VariableHandlers = serviceProvider.GetServices<IResponseVariableParsingHandler>().Select(h =>
                new VariableHandlerModel(h.Name, h.FullName, h.Examples, h.GetDescription())),
            RuntimeVersion = envService.GetRuntime()
        }
            .AsTask();
}
