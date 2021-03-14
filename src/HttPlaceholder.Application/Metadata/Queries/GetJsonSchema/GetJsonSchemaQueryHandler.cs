using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Domain;
using MediatR;
using NJsonSchema;

namespace HttPlaceholder.Application.Metadata.Queries.GetJsonSchema
{
    public class GetJsonSchemaQueryHandler : IRequestHandler<GetJsonSchemaQuery, string>
    {
        public Task<string> Handle(GetJsonSchemaQuery request, CancellationToken cancellationToken)
        {
            var schema = request.AsArray ? JsonSchema.FromType<StubModel[]>() : JsonSchema.FromType<StubModel>();
            return Task.FromResult(schema.ToJson());
        }
    }
}
