using System.Linq;
using HttPlaceholder.Application.StubExecution.OpenAPIParsing.Models;
using Microsoft.OpenApi.Readers;

namespace HttPlaceholder.Application.StubExecution.OpenAPIParsing.Implementations;

/// <inheritdoc />
internal class OpenApiParser : IOpenApiParser
{
    /// <inheritdoc />
    public OpenApiResult ParseOpenApiDefinition(string input)
    {
        // TODO catch exceptions and turn them into ValidationException.
        var openapi = new OpenApiStringReader().Read(input, out _);
        var serverUrl = openapi.Servers.Any() ? openapi.Servers.First().Url : "http://localhost";

        return new OpenApiResult
        {
            ServerUrl = serverUrl,
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
