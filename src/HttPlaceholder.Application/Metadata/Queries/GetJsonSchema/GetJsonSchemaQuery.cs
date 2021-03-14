using MediatR;

namespace HttPlaceholder.Application.Metadata.Queries.GetJsonSchema
{
    public class GetJsonSchemaQuery : IRequest<string>
    {
        public GetJsonSchemaQuery(bool asArray)
        {
            AsArray = asArray;
        }

        public bool AsArray { get; }
    }
}
