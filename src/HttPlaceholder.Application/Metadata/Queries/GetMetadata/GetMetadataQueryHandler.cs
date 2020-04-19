using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;
using MediatR;
// ReSharper disable UnusedType.Global

namespace HttPlaceholder.Application.Metadata.Queries.GetMetadata
{
    public class GetMetadataQueryHandler : IRequestHandler<GetMetadataQuery, MetadataModel>
    {
        private readonly IAssemblyService _assemblyService;

        public GetMetadataQueryHandler(IAssemblyService assemblyService)
        {
            _assemblyService = assemblyService;
        }

        public Task<MetadataModel> Handle(GetMetadataQuery request, CancellationToken cancellationToken) =>
            Task.FromResult(new MetadataModel {Version = _assemblyService.GetAssemblyVersion()});
    }
}
