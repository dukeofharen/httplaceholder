using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Metadata.Queries.GetMetadata
{
    public class GetMetadataQuery : IRequest<MetadataModel>
    {
    }
}
