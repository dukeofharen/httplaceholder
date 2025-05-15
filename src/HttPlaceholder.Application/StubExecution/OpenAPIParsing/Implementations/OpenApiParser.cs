using System.Linq;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.OpenAPIParsing.Models;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace HttPlaceholder.Application.StubExecution.OpenAPIParsing.Implementations;

internal class OpenApiParser : IOpenApiParser, ISingletonService
{
    /// <inheritdoc />
    public OpenApiResult ParseOpenApiDefinition(string input)
    {
        var openapi = new OpenApiStringReader().Read(input, out _);
        return new OpenApiResult
        {
            Server = openapi.Servers.Any() ? openapi.Servers.First() : new OpenApiServer { Url = "http://localhost" },
            Lines = (from path in openapi.Paths
                     from operation in path.Value.Operations
                     from operationResponse in operation.Value.Responses
                     select new OpenApiLine
                     {
                         Operation = operation.Value,
                         Response = operationResponse.Value,
                         OperationType = operation.Key,
                         PathKey = path.Key,
                         ResponseKey = operationResponse.Key
                     }).ToArray()
        };
    }
}
