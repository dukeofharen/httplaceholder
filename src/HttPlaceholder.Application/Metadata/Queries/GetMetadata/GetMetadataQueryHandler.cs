using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.VariableHandling;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Application.Metadata.Queries.GetMetadata
{
    public class GetMetadataQueryHandler : IRequestHandler<GetMetadataQuery, MetadataModel>
    {
        private readonly IAssemblyService _assemblyService;
        private readonly IServiceProvider _serviceProvider;

        public GetMetadataQueryHandler(
            IAssemblyService assemblyService,
            IServiceProvider serviceProvider)
        {
            _assemblyService = assemblyService;
            _serviceProvider = serviceProvider;
        }

        public Task<MetadataModel> Handle(GetMetadataQuery request, CancellationToken cancellationToken)
        {
            var handlers = _serviceProvider.GetServices<IVariableHandler>();
            var result = new MetadataModel
            {
                Version = _assemblyService.GetAssemblyVersion(),
                VariableHandlers = handlers.Select(h => new VariableHandlerModel(h.Name, h.FullName, h.Example))
            };
            return Task.FromResult(result);
        }
    }
}
