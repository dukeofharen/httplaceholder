using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace HttPlaceholder.Application.StubExecution.Implementations;

/// <inheritdoc />
public class OpenApiStubGenerator : IOpenApiStubGenerator
{
    private readonly IHttpRequestToConditionsService _httpRequestToConditionsService;
    private readonly IHttpResponseToStubResponseService _httpResponseToStubResponseService;
    private readonly IStubContext _stubContext;

    public OpenApiStubGenerator(
        IHttpRequestToConditionsService httpRequestToConditionsService,
        IHttpResponseToStubResponseService httpResponseToStubResponseService,
        IStubContext stubContext)
    {
        _httpRequestToConditionsService = httpRequestToConditionsService;
        _httpResponseToStubResponseService = httpResponseToStubResponseService;
        _stubContext = stubContext;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<FullStubModel>> GenerateOpenApiStubs(string input, bool doNotCreateStub)
    {
        // TODO set description
        // TODO catch exceptions and turn them into ValidationException.
        var openapi = new OpenApiStringReader().Read(input, out _);
        var serverUrl = openapi.Servers.Any() ? openapi.Servers.First().Url : "http://localhost";
        var stubs = new List<FullStubModel>();
        foreach (var path in openapi.Paths)
        {
            foreach (var operation in path.Value.Operations)
            {
                foreach (var operationResponse in operation.Value.Responses)
                {
                    var request = new HttpRequestModel
                    {
                        Body = BuildRequestBody(operation.Value),
                        Headers = BuildHeaders(operation.Value),
                        Method = operation.Key.ToString().ToUpper(),
                        Url = $"{serverUrl}{BuildRelativePath(operation.Value, path.Key)}"
                    };
                }
            }
        }

        return stubs;
    }

    private static string BuildRequestBody(OpenApiOperation operation)
    {
        //operation.RequestBody;
        throw new NotImplementedException(); // TODO
    }

    private static string BuildRelativePath(OpenApiOperation operation, string basePath)
    {
        var relativePath = basePath;
        var parameters = operation.Parameters;

        // Parse path parameters
        var pathParams = parameters
            .Where(p => p.In == ParameterLocation.Path)
            .ToArray();
        if (pathParams.Any())
        {
            relativePath = pathParams
                .Aggregate(
                    relativePath,
                    (current, parameter) => current.Replace($"{{{parameter.Name}}}",
                        GetRandomValue(parameter.Schema))); // TODO
        }

        // Parse query parameters.
        var queryParams = parameters
            .Where(p => p.In == ParameterLocation.Query)
            .ToArray();
        if (queryParams.Any())
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            foreach (var param in queryParams)
            {
                queryString.Add(param.Name, GetRandomValue(param.Schema));
            }

            relativePath = $"{relativePath}?{queryString}";
        }

        return relativePath;
    }

    private static IDictionary<string, string> BuildHeaders(OpenApiOperation operation)
    {
        // Parse header parameters.
        var headerParams = operation.Parameters
            .Where(p => p.In == ParameterLocation.Header)
            .ToArray();
        return headerParams.Any()
            ? headerParams.ToDictionary(p => p.Name, p => GetRandomValue(p.Schema))
            : new Dictionary<string, string>();
    }

    private static string GetRandomValue(OpenApiSchema schema)
    {
        // TODO use something like Bogus for this?
        switch (schema.Type)
        {
            case "boolean":
                return true.ToString();
            case "string":
                return Guid.NewGuid().ToString();
            case "integer":
                return 42.ToString();
            case "number":
                return 12.34.ToString(CultureInfo.InvariantCulture);
            default:
                return "x";
        }
    }
}
