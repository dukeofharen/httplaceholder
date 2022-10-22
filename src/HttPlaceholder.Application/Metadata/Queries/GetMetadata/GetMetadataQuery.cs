using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Metadata.Queries.GetMetadata;

/// <summary>
///     A query for retrieving metadata about the currently running instance of HttPlaceholder.
/// </summary>
public class GetMetadataQuery : IRequest<MetadataModel>
{
}
